﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupon.ToListAsync());
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupons)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);

                            p1 = ms1.ToArray();
                        }
                    }
                    coupons.Picture = p1;
                }
                _db.Coupon.Add(coupons);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupons);
        }

        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupons = await _db.Coupon.SingleOrDefaultAsync(m => m.Id == id);

            if (coupons == null)
            {
                return NotFound();
            }

            return View(coupons);

        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Coupon coupons)
        {

            if (coupons.Id == 0)
            {
                return NotFound();
            }

            var couponFromDb = await _db.Coupon.Where(c => c.Id == coupons.Id).FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    couponFromDb.Picture = p1;
                }
                couponFromDb.MinimumAmount = coupons.MinimumAmount;
                couponFromDb.Name = coupons.Name;
                couponFromDb.Discount = coupons.Discount;
                couponFromDb.CouponType = coupons.CouponType;
                couponFromDb.IsActive = coupons.IsActive;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupons);
        }

        //GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupons = await _db.Coupon.SingleOrDefaultAsync(m => m.Id == id);

            if (coupons == null)
            {
                return NotFound();
            }

            return View(coupons);
        }

        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
               return NotFound();
            }

            var coupons = await _db.Coupon.SingleOrDefaultAsync(m => m.Id == id);

            if (coupons == null)
            {
                return NotFound();
            }

            return View(coupons);
        }

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Coupon coupons)
        {
            if (coupons == null)
            {
                return NotFound();
            }

            var couponFromDB = await _db.Coupon.Where(m => m.Id == coupons.Id).FirstOrDefaultAsync();

            if (couponFromDB == null)
            {
                return NotFound();
            }

            _db.Remove(couponFromDB);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



    }
}
