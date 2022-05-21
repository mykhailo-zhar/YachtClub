using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public static class ControllerExtension
    {
        public static void HandleException(this Controller controller, Exception exception)
        {
            if (exception is DbUpdateException dbUpdateEx)
            {
                if (dbUpdateEx.GetBaseException() is PostgresException pgEx)
                {
                    switch (pgEx.SqlState)
                    {
                        case PostgresErrorCodes.UniqueViolation:
                            controller.ModelState.AddModelError("UniqueViolation",
                                "Запись с похожими полями уже существует. Нарушение уникальности!");
                            break;
                        case PostgresErrorCodes.CheckViolation:
                            controller.ModelState.AddModelError("CheckViolation",
                                $"<{pgEx.ConstraintName}> - нарушение!");
                            break;
                        case PostgresErrorCodes.RaiseException:
                            controller.ModelState.AddModelError("Trigger",
                                $"{pgEx.Message[7..]}");
                            break;
                        default:
                            controller.ModelState.AddModelError("Other",
                                $"{pgEx.Message[7..]}");
                            break;
                    }
                }
                
            }
            else
            {
                controller.ModelState.AddModelError("Other",
                           $"{exception.Message}");
            }
        }
    }
}
