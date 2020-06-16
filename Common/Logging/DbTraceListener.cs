using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Logging
{
    /// <summary>
    /// This TraceListener derived class will cause all trace writes to 
    /// be stored in a database table. In order to reduce any performance
    /// impact on the hosting Application all traced messages are stored
    /// in a circular buffer, a worker thread reads the buffer and stores 
    /// the messages in a database table. If for whatever reason the traceListener
    /// is unable to write to the database then it is possible for the tracelistener 
    /// to lose trace messages as the circular buffer is overwritten
    /// </summary>
    public class DbTraceListener : System.Diagnostics.TraceListener
    {
        #region Construction

        string _connectionName = string.Empty;
        const string _defaultApplicationName = "";
        const string _defaultCommandText = "EXEC usp_InsertANDLog " +
            " @category,@priority,@message,@callStack,@source,@process,@threadId,@userId, " +
            "@machineName,@loggedTimeStamp;";


        const int _defaultMaxMessageLength = 1500;

        private const string TRACE_SWITCH_NAME = "DBTraceSwitch";

        private const string TRACE_SWITCH_DESCRIPTION = "Trace switch defined in config file for configuring trace output to database";

        // Not defining it as readonly string so that in future it could come
        // from an external source and we can provide initializer for it
        private static readonly string DEFAULT_TRACE_TYPE = "Verbose";


        // Trace Switch object for controlling trace output, defaulting to Verbose
        private TraceSwitch TraceSwitch = new TraceSwitch(TRACE_SWITCH_NAME, TRACE_SWITCH_DESCRIPTION, DEFAULT_TRACE_TYPE);

        // Lock object
        private object _traceLockObject = new object();
        private object _fileLockObject = new object();

        private static string[] _supportedAttributes = new string[]
            {
                "ApplicationName", "ApplicationName", "Applicationname",
                "commandText", "CommandText", "commandtext",
                "maxMessageLength", "MaxMessageLength", "maxmessagelength",
            };

        /// <summary>
        /// Gets or sets the name of the Application used when logging to the database.
        /// </summary>
        public string ApplicationName
        {
            get
            {
                // Default format matches System.Diagnostics.TraceListener
                if (Attributes.ContainsKey("Applicationname"))
                {
                    return Attributes["Applicationname"];
                }
                else
                {
                    return _defaultApplicationName;
                }
            }
            set
            {
                Attributes["Applicationname"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the command, with parameters, sent to the database.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default command text calls the diagnostics_Trace_AddEntry stored
        /// procedure, created by the diagnostics_regsql tool.
        /// </para>
        /// <para>
        /// To bypass the stored procedure, you can directly insert by setting
        /// the command text to something like
        /// "INSERT INTO dbo.diagnostics_Trace ( ApplicationName, Source, Id, EventType, UtcDateTime, MachineName, AppDomainFriendlyName, ProcessId, ThreadName, Message, ActivityId, RelatedActivityId, LogicalOperationStack, Data ) VALUES ( @ApplicationName, @Source, @Id, @EventType, @UtcDateTime, @MachineName, @AppDomainFriendlyName, @ProcessId, @ThreadName, @Message, @ActivityId, @RelatedActivityId, @LogicalOperationStack, @Data )".
        /// </para>
        /// </remarks>
        public string CommandText
        {
            get
            {
                // Default format matches System.Diagnostics.TraceListener
                if (Attributes.ContainsKey("commandtext"))
                {
                    return Attributes["commandtext"];
                }
                else
                {
                    return _defaultCommandText;
                }
            }
            set
            {
                Attributes["commandtext"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the length to trim any message text to.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is the maximum length of the message text to write to the database,
        /// where the database column has limited size. Messages (after inserting
        /// format parameters) are trimmed to this length, with the last three
        /// characters replaced by "..." if the original message was longer.
        /// </para>
        /// <para>
        /// A value of zero (0) can be used to remove the message limit length,
        /// for example where the column has no limit on length (e.g. a
        /// column of type ntext).
        /// </para>
        /// </remarks>
        public int MaxMessageLength
        {
            get
            {
                if (Attributes.ContainsKey("maxmessagelength"))
                {
                    int value;
                    if (!int.TryParse(Attributes["maxmessagelength"], out value))
                    {
                        value = _defaultMaxMessageLength;
                    }
                    return value;
                }
                else
                {
                    return _defaultMaxMessageLength;
                }
            }
            set
            {
                Attributes["maxmessagelength"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets the name of the connection string that identifies the database to use.
        /// </summary>
        public string ConnectionName
        {
            get { return _connectionName; }
        }

        /// <summary>
        /// Allowed attributes for this trace listener.
        /// </summary>
        protected override string[] GetSupportedAttributes()
        {
            return _supportedAttributes;
        }

        public DbTraceListener(string connectionName)
        {
            this._connectionName = connectionName;
            this._sqlProc = CommandText;
            _worker = new Thread(new ThreadStart(Worker));
            _worker.Start();
            Open(_connectionName);
        }

        public DbTraceListener()
        {

        }

        #endregion Construction

        #region TraceListener Overrides

        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        public override void Close()
        {
            base.Close();
            try
            {
                if (this._cxn != null && this._cxn.State != ConnectionState.Open)
                    this._cxn.Close();
            }
            catch { }
        }

        public override void Flush()
        {
            Poke();
            try
            {
                _queueDrainEvent.WaitOne(new TimeSpan(0, 0, 0, 5), false);
            }
            catch (Exception)
            {
            }

        }

        public override string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        internal bool ShouldLogTrace(TraceEventType eventType)
        {
            bool shouldLog = true;

            switch (eventType)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    shouldLog = this.TraceSwitch.TraceError;
                    break;

                case TraceEventType.Warning:
                    shouldLog = this.TraceSwitch.TraceWarning;
                    break;

                case TraceEventType.Information:
                    shouldLog = this.TraceSwitch.TraceInfo;
                    break;

                case TraceEventType.Start:
                case TraceEventType.Stop:
                case TraceEventType.Suspend:
                case TraceEventType.Resume:
                case TraceEventType.Transfer:
                case TraceEventType.Verbose:
                    shouldLog = this.TraceSwitch.TraceVerbose;
                    break;
            }

            return shouldLog;
        }

        #region TraceEvent

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            this.TraceEvent(eventCache, source, eventType, id, string.Empty);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {

            try
            {
                if (!this.ShouldLogTrace(eventType))
                    return;

                if (eventType == TraceEventType.Critical ||
                        eventType == TraceEventType.Error ||
                        eventType == TraceEventType.Warning)
                {


                    WriteTrace(eventType.ToString(), message, eventCache.Callstack.ToString(),
                          eventCache.ThreadId, source, "", 0, "", "", "", 0, eventCache.DateTime);

                }
                else
                {
                    WriteTrace(eventType.ToString(), message, "",
                       eventCache.ThreadId, source, "", 0, "", "", "", 0, eventCache.DateTime);
                }

            }
            catch (Exception ex)
            {
                this.WriteLine(
                    string.Format("AND::DbTraceListener - Trace.TraceTransfer failed with following exception: {0}, for message {1} ", ex.ToString(), message),
                    "Error",
                    "DBTraceListener"
                );

                this.WriteEntryToInternalLog(string.Format("Trace.TraceTransfer failed with following exception: {0}", ex.ToString()));
            }
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            try
            {
                if (!this.ShouldLogTrace(eventType))
                    return;

                if (eventType == TraceEventType.Critical ||
                        eventType == TraceEventType.Error ||
                        eventType == TraceEventType.Warning)
                {

                    WriteTrace(eventType.ToString(), string.Format(format, args), eventCache.Callstack.ToString(),
                        eventCache.ThreadId, source, "", 0, "", "", "", 0, eventCache.DateTime);

                }
                else
                {
                    WriteTrace(eventType.ToString(), string.Format(format, args), "",
                       eventCache.ThreadId, source, "", 0, "", "", "", 0, eventCache.DateTime);
                }

            }

            catch (Exception ex)
            {
                this.WriteLine(
                    string.Format("AND::DbTraceListener - Trace.TraceTransfer failed with following exception: {0}, for message {1} ", ex.ToString(), string.Format(format, args)),
                    "Error",
                    "DBTraceListener"
                );

                this.WriteEntryToInternalLog(string.Format("Trace.TraceTransfer failed with following exception: {0}", ex.ToString()));
            }
        }



        #endregion


        public override void Write(object o)
        {
            WriteTrace("Unknown", o.ToString(), "", GetThreadIdentifier());
        }

        public override void Write(object o, string category)
        {
            WriteTrace(category, o.ToString(), "", GetThreadIdentifier());
        }

        public override void Write(string message)
        {
            WriteTrace("Unknown", message, "", GetThreadIdentifier());
        }

        public override void Write(string message, string category)
        {
            WriteTrace(category, message, "", GetThreadIdentifier());
        }

        public override void WriteLine(object o)
        {
            WriteTrace("Unknown", o.ToString() + "\n", "", GetThreadIdentifier());
        }

        public override void WriteLine(object o, string category)
        {
            WriteTrace(category, o.ToString() + "\n", "", GetThreadIdentifier());
        }

        public override void WriteLine(string message)
        {
            WriteTrace("Unknown", message + "\n", "", GetThreadIdentifier());
        }

        public void WriteLine(string message, string callStack, string module)
        {
            WriteTrace("Unknown", message + "\n", callStack + "\n", GetThreadIdentifier(), module);
        }

        public override void WriteLine(string message, string category)
        {
            WriteTrace(category, message + "\n", "", GetThreadIdentifier());
        }



        /// <summary>
        /// Adds a trace message containing details of an exception at the "Error" trace level.
        /// Optionally includes the stack dump
        /// </summary>
        /// <remarks>
        /// No exceptions propagate from this method!
        /// The contents of inner exceptions are dumped as well.
        /// </remarks>
        /// <param name="e">The exception whose content is to be traced.</param>
        /// <param name="bDumpStack">Whether or not to dump the stack associated with the exception.</param>
        public void WriteException(Exception e, bool bDumpStack, string Module, DateTime date)
        {
            try
            {
                StringBuilder str = new StringBuilder(" ");
                str.Append(e.GetType().ToString());
                str.Append(" - ");
                str.Append(e.Message);
                while (e.InnerException != null)
                {
                    str.Append(" -> ");
                    str.Append(e.InnerException.Message);
                    e = e.InnerException;
                }
                if (bDumpStack)
                {
                    WriteTrace(e.GetType().ToString(), str.ToString(), e.StackTrace.ToString(), GetThreadIdentifier(), Module, "", 0, "", "", "", 1, date);
                }
                else
                {
                    WriteTrace(e.GetType().ToString(), str.ToString(), "", GetThreadIdentifier(), "", "", 0, "", "", "", 1, date);
                }

            }
            catch (Exception ex)
            {

                this.WriteLine(
                     string.Format("AND::DbTraceListener - WriteException method failed with following exception: {0}, for method {1} ", ex.ToString(), "WriteException()"),
                     "Error",
                     "DBTraceListener"
                 );

                this.WriteEntryToInternalLog(string.Format("WriteException method failed with following exception: {0}", ex.ToString()));
            }
        }

        public void WriteException(Exception e, bool bDumpStack)
        {
            WriteException(e, bDumpStack, "Exception", DateTime.Now);
        }

        protected override void Dispose(bool disposing)
        {
            if (_worker != null)
            {
                // give it up to 5 seconds to drain the queue and then kill it 
                try
                {
                    _queueDrainEvent.WaitOne(new TimeSpan(0, 0, 0, 5), false);
                }
                catch (Exception) // don't care whether it succeeded or not as im going to kill it whatever happens
                {

                }
                _worker.Abort();
            }
        }

        #endregion  TraceListener Overrides

        #region Private helper methods

        /// <summary>
        /// Thread function which empties the queue when woken by the work event 
        /// The QueueDrainedEvent is set when the Queue is emptied
        /// </summary>
        private void Worker()
        {
            for (;;)
            {
                if (_workEvent.WaitOne())
                {
                    if (_cxn == null)
                    {
                        Open(_connectionName);
                    }

                    if (_cxn != null)
                    {
                        SqlCommand cmd = new SqlCommand(_sqlProc, _cxn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = CommandText;
                        SqlParameter pCategory = cmd.Parameters.Add("@category", SqlDbType.NVarChar);
                        SqlParameter pPriority = cmd.Parameters.Add("@priority", SqlDbType.Int);
                        SqlParameter pMessage = cmd.Parameters.Add("@message", SqlDbType.NVarChar);
                        SqlParameter pCallStack = cmd.Parameters.Add("@callStack", SqlDbType.NVarChar);
                        SqlParameter pSource = cmd.Parameters.Add("@source", SqlDbType.NVarChar);
                        SqlParameter pProcess = cmd.Parameters.Add("@process", SqlDbType.NVarChar);
                        SqlParameter pThreadId = cmd.Parameters.Add("@threadId", SqlDbType.NVarChar);
                        SqlParameter pUserId = cmd.Parameters.Add("@userId", SqlDbType.NVarChar);
                        SqlParameter pMachineName = cmd.Parameters.Add("@machineName", SqlDbType.NVarChar);
                        SqlParameter pLoggedTimeStamp = cmd.Parameters.Add("@loggedTimeStamp", SqlDbType.DateTime);

                        MsgEntry msg = null;


                        while ((msg = _msgs.Next()) != null)
                        {
                            pCategory.Value = msg._category;
                            pPriority.Value = msg._priority;
                            pMessage.Value = msg._message;
                            pCallStack.Value = msg._callStack;
                            pSource.Value = msg._source;

                            string Process = string.Format("{0} {1}",
                            System.Diagnostics.Process.GetCurrentProcess().ProcessName,
                            System.Diagnostics.Process.GetCurrentProcess().Id);

                            pProcess.Value = Process;
                            pThreadId.Value = msg._threadId;
                            pUserId.Value = msg._userId;
                            pMachineName.Value = msg._machineName;
                            pLoggedTimeStamp.Value = msg._loggedDate;

                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                this.WriteLine(
                                   string.Format("AND::DbTraceListener - Worker method failed with following exception: {0}, for method {1} ", ex.ToString(), "Worker()"),
                                   "Error",
                                   "DbTraceListener"
                               );

                                this.WriteEntryToInternalLog(string.Format("Open connection failed with following exception: {0}", ex.ToString()));
                            }
                        }
                        // Queue is empty - ok to dispose
                        _queueDrainEvent.Set();
                    }
                }
            }
        }
        /// <summary>
        /// Gets a string representing the identifier of the current thread.
        /// The thread's name is used if it has one, otherwise the thread's
        /// id integer is converted to a string and returned.
        /// </summary>
        /// <returns>String representation of the current thread's identity.</returns>
        private string GetThreadIdentifier()
        {
            string strThread = Thread.CurrentThread.Name;
            if (strThread == null || strThread.Length == 0)
            {
                strThread = Thread.CurrentThread.ManagedThreadId.ToString();
            }
            return strThread;
        }

        /// <summary>
        /// Creates an entry in the buffer containing a message to be written to the database.
        /// </summary>
        /// <param name="message">Text for the message.</param>
        /// <param name="module">Where the message came from.  Often the name of the traceswitch.</param>
        /// <param name="thread">The thread from whence the message came.</param>
        private void WriteTrace(string category, string message, string callStack, string threadId,
            string source = "", string process = "", int priority = 0, string module = "", string userId = "",
            string machineName = "", int verbose = 0, DateTime? loggedDate = null)
        {
            _msgs.AddMsgEntry(new MsgEntry(category, message, callStack, threadId, source, process, priority,
                module, userId, machineName, verbose, loggedDate));
            Poke();
        }


        private void WriteEntryToInternalLog(string msg)
        {
            lock (this._fileLockObject)
            {
                try
                {
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\DBTraceListener.log",
                        string.Format("{0}{1}: {2}", Environment.NewLine, DateTime.Now.ToString(), msg));
                }
                catch (Exception ex)
                {
                    this.WriteLine(
                    string.Format("AND::DbTraceListener - WriteEntryToInternalLog method failed with following exception: {0}, for method {1} ", ex.ToString(), "WriteEntryToInternalLog"),
                    "Error",
                    "DBTraceListener"
                );

                }
            }
        }

        /// <summary>
        /// Sets the work event to wake up the worker thread and resets the _queueDrainEvent
        /// to signify that there is data still to be written to the database
        /// </summary>
        internal void Poke()
        {
            _queueDrainEvent.Reset();
            _workEvent.Set();
        }


        private void Open(string connectionstring)
        {
            try
            {
                try
                {
                    if (!string.IsNullOrEmpty(connectionstring))
                    {
                        _cxn = new SqlConnection(connectionstring);
                        _cxn.Open();
                    }
                }
                catch (Exception ex)
                {
                    this.WriteLine(
                        string.Format("AND::DbTraceListener - Open connection failed with following exception: {0}, for method {1} ", ex.ToString(), "Open -- Inner catch"),
                        "Error",
                        "DbTraceListener"
                    );

                    this.WriteEntryToInternalLog(string.Format("Open connection failed with following exception: {0}", ex.ToString()));
                }
            }
            catch (Exception ex)
            {
                _cxn = null;
                this.WriteLine(
                        string.Format("AND::DbTraceListener - Open connection failed with following exception: {0}, for method {1} ", ex.ToString(), "Open--outer catch"),
                        "Error",
                        "DbTraceListener"
                    );

                this.WriteEntryToInternalLog(string.Format("Open connection failed with following exception: {0}", ex.ToString()));
            }
        }


        #endregion Private helper methods

        #region private instance data

        SqlConnection _cxn = null;
        string _dbName = null;
        string _dbHost = null;
        string _sqlProc = null;
        string _name = "DbTraceListener";
        CircularBuffer _msgs = new CircularBuffer();
        Thread _worker = null;
        AutoResetEvent _workEvent = new AutoResetEvent(false);
        ManualResetEvent _queueDrainEvent = new ManualResetEvent(false);

        #endregion private instance data
    }
    #region MsgEntry

    public class MsgEntry : IEquatable<MsgEntry>
    {
        public MsgEntry(string category, string message)
        {
            _category = category;
            _message = message;
            _threadId = "Unknown";
            _callStack = "";
            _loggedDate = null;
        }
        public MsgEntry(string category, string message, string threadId)
            : this(category, message)
        {
            _threadId = threadId;
        }
        public MsgEntry(string category, string message, string callStack, string threadId,
            string source = "", string process = "", int priority = 0, string module = "", string userId = "",
            string machineName = "", int verbose = 0, DateTime? loggedDate = null)
        {
            _category = category;
            _priority = priority;
            _message = message;
            _callStack = callStack;
            _source = source;
            _threadId = threadId;
            _userId = userId;
            _machineName = machineName;
            _module = module;
            _verbose = verbose;
            _loggedDate = loggedDate;
        }
        public MsgEntry() { }

        public string _module;
        public string _detail;
        public int _verbose = 0;

        public string _category;
        public int _priority;
        public string _message;
        public string _callStack;
        public string _source;
        public string _process;
        public string _threadId;
        public string _userId;
        public Guid _guid;
        public string _machineName;
        public DateTime? _loggedDate;



        public bool Equals(MsgEntry other)
        {
            if (this._message == other._message && this._callStack == other._callStack)
                return true;
            return false;
        }
    }

    #endregion MsgEntry

    #region CircularBuffer
    /// <summary>
    /// create buffer class to store message
    /// </summary>
    public class CircularBuffer
    {
        #region public methods

        private MsgEntry _last = new MsgEntry();
        /// <summary>
        /// Adds an entry to the buffer.
        /// </summary>
        /// <param name="msg">The entry to be added.</param>
        public void AddMsgEntry(MsgEntry msg)
        {

            lock (_msgs.SyncRoot)
            {
                //if (!_last.Equals(msg))
                {
                    _msgs.Enqueue(msg);

                    if (_msgs.Count >= _maxSize)
                    {
                        while (_msgs.Count >= _maxSize)
                        {
                            // TODO : add in the relevant API call to send the discard message to the default debug stream
                            // OutputDebugString(.......
                            _msgs.Dequeue();
                        }
                    }
                }
                _last = msg;
            }
        }

        /// <summary>
        /// Fetches the oldest entry in the queue.
        /// </summary>
        /// <returns>The entry fetched.</returns>
        public MsgEntry Next()
        {
            lock (_msgs.SyncRoot)
            {
                if (_msgs.Count == 0)
                    return null;
                return (MsgEntry)_msgs.Dequeue();
            }
        }

        /// <summary>
        /// The number of messages currently in the queue.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_msgs.SyncRoot)
                {
                    return _msgs.Count;
                }
            }
        }

        #endregion public methods

        #region Properties

        /// <summary>
        /// The maximum size allowed for the buffer.
        /// </summary>
        public int MaxSize
        {
            get { return _maxSize; }
            set { _maxSize = value; }
        }

        #endregion Properties

        #region Private data

        Queue _msgs = new Queue();
        int _maxSize = 500;

        #endregion Private data
    }
    #endregion
}
