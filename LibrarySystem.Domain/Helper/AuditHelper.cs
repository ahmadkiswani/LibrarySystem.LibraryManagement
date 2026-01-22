
using LibrarySystem.Entities.Models;

namespace LibrarySystem.Domain.Helper
{

   
    
        public static class AuditHelper
        {
            public static void OnCreate(AuditLog entity)
            {
                entity.CreatedDate = DateTime.UtcNow;
                entity.IsDeleted = false;
            }

            public static void OnUpdate(AuditLog entity, int? userId)
            {
                entity.LastModifiedDate = DateTime.UtcNow;
                entity.LastModifiedBy = userId;
            }

            public static void OnSoftDelete(AuditLog entity, int? userId)
            {
                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;
                entity.DeletedBy = userId;
            }
        }
   }

    
