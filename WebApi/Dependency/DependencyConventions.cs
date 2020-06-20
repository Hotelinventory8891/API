//using Castle.Facilities.Logging;
using Common.Logging;
using Common.Translators;
using Common.Validator;
using Core;
using DAL.Contract;
using DAL;
using Repository;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web;
using System;
using Repository.LookUp;
using Core.Lookup;
using Core.Property;
using Repository.Property;

namespace Dependency
{
    public class DependencyConventions : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().
                BasedOn<ApiController>().LifestyleTransient());
            container.Register(Classes.FromThisAssembly()
                               .BasedOn<IController>()
                               .LifestyleTransient());
            container.Register(
                  Component.For<IQueryableUnitOfWork, UnitOfWork>().ImplementedBy<UnitOfWork>().LifestylePerWebRequest(),
                  Component.For<ILogger, TraceSourceLog>().ImplementedBy<TraceSourceLog>().LifestyleSingleton(),
                  Component.For<IEntityTranslatorService, EntityTranslatorService>().ImplementedBy<EntityTranslatorService>().LifestyleSingleton(),
                  Component.For<ILookupTypeRepository, LookupTypeRepository>().ImplementedBy<LookupTypeRepository>().LifestyleTransient(),
                  Component.For<ILookupRepository, LookupRepository>().ImplementedBy<LookupRepository>().LifestyleTransient(),
                  Component.For<ILookupViewRepository, LookupViewRepository>().ImplementedBy<LookupViewRepository>().LifestyleTransient(),
                  Component.For<IUserRepository, UserRepository>().ImplementedBy<UserRepository>().LifestyleTransient(),
                  Component.For<IPropertyUserRepository, PropertyUserRepository>().ImplementedBy<PropertyUserRepository>().LifestyleTransient(),
                  Component.For<IPropertyRepository, PropertyRepository>().ImplementedBy<PropertyRepository>().LifestyleTransient(),
                  Component.For<IPropertyFacilityRepository, PropertyFacilityRepository>().ImplementedBy<PropertyFacilityRepository>().LifestyleTransient(),
                  Component.For<IPropertyDetailsViewRepository, PropertyDetailsViewRepository>().ImplementedBy<PropertyDetailsViewRepository>().LifestyleTransient(),
                  Component.For<IRoomRepository, RoomRepository>().ImplementedBy<RoomRepository>().LifestyleTransient(),
                  Component.For<IRoomFacilityRepository, RoomFacilityRepository>().ImplementedBy<RoomFacilityRepository>().LifestyleTransient(),
                  Component.For<IRoomTariffRepository, RoomTariffRepository>().ImplementedBy<RoomTariffRepository>().LifestyleTransient(),
                  Component.For<IRoomDetailsViewRepository, RoomDetailsViewRepository>().ImplementedBy<RoomDetailsViewRepository>().LifestyleTransient()

                );
            //.AddFacility<LoggingFacility>(f => f.UseLog4Net());


            LoggerFactory.SetCurrent(new TraceSourceLogFactory());
            EntityValidatorFactory.SetCurrent(new DataAnnotationsEntityValidatorFactory());

            IEntityTranslatorService translatorService = container.Kernel.Resolve<IEntityTranslatorService>();
            RegisterTranslators(translatorService);

        }

        public void RegisterTranslators(IEntityTranslatorService translatorService)
        {
            //translatorService.RegisterEntityTranslator(new EnumerationTranslator());
        }
    }
}