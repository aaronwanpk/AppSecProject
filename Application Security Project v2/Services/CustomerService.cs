using Application_Security_Project_v2.Models;
using Microsoft.EntityFrameworkCore;

namespace Application_Security_Project_v2.Services
{
    public class CustomerService
    {
        private readonly MyDbContext _context;
        public CustomerService(MyDbContext context)
        {
            _context = context;
        }
        public List<Customer> GetAll()
        {
            return _context.Customers.OrderBy(m => m.FName).ToList();
        }
        public Customer? GetCustomerByEmail(string email)
        {
            Customer? customer = _context.Customers.FirstOrDefault(x => x.Email.Equals(email));
            return customer;
        }

        public Customer? GetCustomerByPassword(string password)
        {
            Customer? customer = _context.Customers.FirstOrDefault(x => x.PasswordHash.Equals(password));
            return customer;
        }
        public void AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }
        public void UpdateEmployee(Customer customer)
        {
            _context.Customers.Update(customer);
            _context.SaveChanges();
        }
        public void DeleteAllCustomer() 
        { 
            _context.Customers.ExecuteDelete();
            _context.SaveChanges();
        }
    }
}
