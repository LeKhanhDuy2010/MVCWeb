﻿using E_CommerceWeb.Data;
using E_CommerceWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceWeb.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly EcommerceDbContext _context;

        public HangHoaController(EcommerceDbContext context) => _context = context;
        public IActionResult Index(int? Loai)
        {
            var hangHoas = _context.HangHoas.Include(p => p.MaLoaiNavigation).AsQueryable();
            if (Loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == Loai.Value);

            }
            var result = hangHoas.Select(hh => new HangHoaVM
            {
                MaHh = hh.MaHh,
                TenHh = hh.TenHh,
                Hinh = hh.Hinh ?? "",
                DonGia = hh.DonGia ?? 0,
                MoTaNgan = hh.MoTaDonVi ?? "",
                TenLoai = hh.MaLoaiNavigation.TenLoai,
            }).ToList();
            return View(result);
        }

        public IActionResult Search(string? query)
        {
            var hangHoas = _context.HangHoas.AsQueryable();
            if(query != null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHh = p.TenHh,
                Hinh = p.Hinh ?? "",
                DonGia = p.DonGia ?? 0,
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai,
            });

            return View(result);
        }
    }
}
