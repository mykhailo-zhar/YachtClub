using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Project.Migrations;

namespace Project.Models
{
    public class Translator
    {
        public static RepairStaff FromStaffPosition(StaffPosition staffPosition) => new RepairStaff { 
            Id = staffPosition.Id,
            Startdate =  staffPosition.Startdate,
            Enddate = staffPosition.Enddate,
            Description = staffPosition.Description,
            Staffid = staffPosition.Staffid,
            Positionid = staffPosition.Positionid            
        };
        public static StaffPosition FromStaffPosition(RepairStaff staffPosition) => new StaffPosition { 
            Id = staffPosition.Id.Value,
            Startdate =  staffPosition.Startdate.Value,
            Enddate = staffPosition.Enddate.Value,
            Description = staffPosition.Description,
            Staffid = staffPosition.Staffid.Value,
            Positionid = staffPosition.Positionid.Value           
        };
    }
}
