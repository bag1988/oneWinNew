using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using oneWin.Data;
using oneWin.Models.baseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Controllers
{
    public class PaymentController : Controller
    {
        private oneWinDbContext _context;

        public PaymentController(oneWinDbContext context)
        {
            _context = context;
        }
        public ActionResult Index(string searchName = "", bool legal = false)
        {
            if (searchName == null)
                searchName = "";

            var doc = _context.BufPaymentAccount.Where(x => x.DocReg.IP == legal && x.payment.Name.Contains(searchName)).Select(x => x.payment).Distinct().ToList();
            var g = _context.PaymentAccount.Where(x => !_context.BufPaymentAccount.Select(x => x.IdPayment).Contains(x.Id) && x.Name.Contains(searchName)).ToList();
            doc.AddRange(g);
            return View(doc);
        }

        [HttpPost]
        public ActionResult viewPartial(int idDoc)
        {
            ViewBag.idDoc = idDoc;
            var docReg = _context.BufPaymentAccount.Where(x => x.IdPayment == idDoc).Select(x => x.DocReg).ToList();
            return PartialView(docReg);
        }


        public ActionResult Create(int idDoc)
        {
            if (idDoc !=0)
                return View(_context.PaymentAccount.First(x => x.Id == idDoc));
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(paymentAccountModel doc)
        {            
            if (ModelState.IsValid)
            {
                if (doc.Id == 0)
                {
                    await _context.PaymentAccount.AddAsync(doc);
                }
                else
                {
                    _context.PaymentAccount.Update(doc);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }
            return View(doc);
        }
       
        public ActionResult Delete(int idDoc)
        {
            if (idDoc == 0)
                RedirectToAction("index");
            var doc = _context.PaymentAccount.FirstOrDefault(x => x.Id == idDoc);
            if (doc == null)
                return NoContent();
            return View(doc);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int idDoc, string passswordAdmin)
        {
            var doc = _context.PaymentAccount.First(x => x.Id == idDoc);

            _context.PaymentAccount.Remove(doc);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<IActionResult> addAdminProc(int idDoc, List<Guid?> idDocReg)
        {
            var docsList = _context.BufPaymentAccount.Where(x => x.IdPayment == idDoc).ToList();

            var deleteDoc = docsList.Where(x => !idDocReg.Contains(x.IdDoc)).ToList();
            if (deleteDoc.Count > 0)
            {
                _context.BufPaymentAccount.RemoveRange(deleteDoc);
            }

            var docs = docsList.Select(x => x.IdDoc).ToList();
            foreach (Guid g in idDocReg)
            {
                if (!docs.Contains(g))
                {
                    await _context.BufPaymentAccount.AddAsync(new bufPaymentAccountModel { IdPayment = idDoc, IdDoc = g });
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
