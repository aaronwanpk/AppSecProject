using Application_Security_Project_v2.Models;
using Microsoft.EntityFrameworkCore;

namespace Application_Security_Project_v2.Services
{
    public class AuditService
    {
        private readonly MyDbContext _context;
        public AuditService(MyDbContext context)
        {
            _context = context;
        }
        public List<Audit> GetAll()
        {
            return _context.AuditEntry.OrderBy(m => m.actionTime).ToList();
        }
        
        public void AddAuditEntry(Audit auditentry)
        {
            _context.AuditEntry.Add(auditentry);
            _context.SaveChanges();
        }
        
    }
}


