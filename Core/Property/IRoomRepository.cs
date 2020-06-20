using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Property
{
    public interface IRoomRepository
    {
        IEnumerable<RoomSnapshot> GetFilteredRoomDetails(Expression<Func<RoomSnapshot, bool>> expr1);
        IEnumerable<RoomSnapshot> GetAllRoomDetails();
        long AddRoom(RoomSnapshot room, bool Commit = true);
        bool UpdateRoom(RoomSnapshot room, bool Commit = true);
        bool DeleteRoom(RoomSnapshot property, bool Commit);
    }
    public interface IRoomFacilityRepository
    {
        IEnumerable<RoomFacilitySnapshot> GetFilteredRoomFacilityDetails(Expression<Func<RoomFacilitySnapshot, bool>> expr1);
        IEnumerable<RoomFacilitySnapshot> GetAllRoomFacilityDetails();
        long AddRoomFacility(RoomFacilitySnapshot room, bool Commit = true);
        bool UpdateRoomFacility(RoomFacilitySnapshot room, bool Commit = true);
        bool DeleteRoomFacility(RoomFacilitySnapshot room, bool Commit = true);
    }
    public interface IRoomTariffRepository
    {
        IEnumerable<RoomTariffSnapshot> GetFilteredRoomTariffDetails(Expression<Func<RoomTariffSnapshot, bool>> expr1);
        IEnumerable<RoomTariffSnapshot> GetAllRoomTariffDetails();
        long AddRoomTariff(RoomTariffSnapshot room, bool Commit = true);
        //bool UpdateRoomTariff(RoomTariffSnapshot room, bool Commit = true);
        //bool DeleteRoomTariff(RoomTariffSnapshot room, bool Commit = true);
    }
    public interface IRoomDetailsViewRepository
    {
        IEnumerable<RoomDetailsViewSnapshot> GetFilteredRoomDetailsViewDetails(Expression<Func<RoomDetailsViewSnapshot, bool>> expr1);
        IEnumerable<RoomDetailsViewSnapshot> GetAllRoomDetailsViewDetails();
    }
}
