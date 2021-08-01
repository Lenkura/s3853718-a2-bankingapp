using WebAPI.Data;
using WebAPI.Models;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Models.Repository;

namespace WebAPI.Models.DataManger
{
    public class BillPayManager : IBankDataRepository<BillPay, int>
    {
        private readonly MCBAContext _context;

        public BillPayManager(MCBAContext context)
        {
            _context = context;
        }
        public BillPay Get(int id)
        {
            return _context.BillPays.Find(id);
        }

        public IEnumerable<BillPay> GetAll()
        {
            return _context.BillPays.ToList();
        }

        public int Add(BillPay billpay)
        {
            _context.BillPays.Add(billpay);
            _context.SaveChanges();

            return billpay.BillPayID;
        }

        public int Delete(int id)
        {
            if (_context.BillPays.Find(id) != null)
            {
                _context.BillPays.Remove(_context.BillPays.Find(id));
                _context.SaveChanges();
            }

            return id;
        }

        public int Update(int id, BillPay billpay)
        {
            _context.Update(billpay);
            _context.SaveChanges();
            return id;
        }
    }
}
