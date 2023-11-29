using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PsbankTest2.Models;

namespace PsbankTest2.Controllers
{
    public class ProductController : Controller
    {
        private PsbankTestDbEntities6 db = new PsbankTestDbEntities6();

        // GET: Product
        public async Task<ActionResult> Index()
        {
            ProductVM viewModel = new ProductVM();
            viewModel.Products = await db.Products.ToListAsync();
            viewModel.OrderList = await db.OrderLists.ToListAsync();
            int total = 0;
            foreach (var item in viewModel.OrderList)
            {
                item.Product = await db.Products.FirstOrDefaultAsync(s => s.Id == item.ProductId);
                total += item.TotalQuantity * item.Product.Cost;
            }
            viewModel.TotalCost = total;
            return View(viewModel);
        }

        // GET: Product/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Cost,TotalQuantity")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Cost,TotalQuantity")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
           var orderlist =  await db.OrderLists.FindAsync(id);
             db.OrderLists.Remove(orderlist);
            await db.SaveChangesAsync();


            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Search(int id) 
        {
            ProductVM viewModel = new ProductVM();
            viewModel.Product = await db.Products.FindAsync(id);
            return PartialView("Form", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> AddOrderList(int productId, int qty)
        {
            OrderList od = new OrderList();
            od.TotalQuantity = qty;
            od.ProductId = productId;
            db.OrderLists.Add(od);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<ActionResult> Transact(int TotalAmount, int Cash)
        {
            try
            {
                Transaction ts = new Transaction();
                ts.TotalAmount = TotalAmount;
                ts.Cash = Cash;
                db.Transactions.Add(ts);
                await db.SaveChangesAsync();

                ProductVM viewModel = new ProductVM();
                viewModel.Transactions = await db.Transactions.ToListAsync();
                return Json(viewModel);
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
