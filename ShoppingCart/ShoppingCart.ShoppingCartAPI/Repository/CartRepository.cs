﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.ShoppingCartAPI.DBContext;
using ShoppingCart.ShoppingCartAPI.Models;
using ShoppingCart.ShoppingCartAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.ShoppingCartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDBContext _db;
        private IMapper _mapper;

        public CartRepository(ApplicationDBContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<bool> ClearCart(string userId)
        {
            var cartHeaderFromDb = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
            if(cartHeaderFromDb != null)
            {
                _db.CartDetails.RemoveRange(_db.CartDetails.Where(u => u.CartHeaderId == cartHeaderFromDb.CartHeaderId));
                _db.CartHeaders.Remove(cartHeaderFromDb);
                await _db.SaveChangesAsync();
                return true;
            }
            else {
                return false;
            }
        }

        public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
        {
            Cart cart = _mapper.Map<Cart>(cartDto);


            // check if product id exists in database,if not create it.

            var prodIndb = _db.Products.FirstOrDefault(u => u.ProductId == cartDto.CartDetails.FirstOrDefault().ProductId);
            if(prodIndb == null)
            {
                _db.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _db.SaveChangesAsync();
            }

            // Check if header is null
            var cartHeaderFromDB = _db.CartHeaders.AsNoTracking().FirstOrDefault(u => u.UserId == cart.CartHeader.UserId);
            if(cartHeaderFromDB == null)
            {
                //create heaeder and details
                _db.CartHeaders.Add(cart.CartHeader);
                await _db.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null;
                _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _db.SaveChangesAsync();

            }
            else
            {
                // if header is not null
                //check if detail has same product
                var cartDetailsFromDB = _db.CartDetails.AsNoTracking().FirstOrDefault(u => u.ProductId == cart.CartDetails.FirstOrDefault().ProductId 
                && u.CartHeaderId == cartHeaderFromDB.CartHeaderId);
                if(cartDetailsFromDB == null)
                {
                    // else create details
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderFromDB.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();

                }
                else
                {
                    //if it has then update the count
                    cart.CartDetails.FirstOrDefault().Count += cartDetailsFromDB.Count;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _db.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
            }
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId)
            };
            cart.CartDetails = _db.CartDetails.Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId).Include(u => u.Product);

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> RemoveFromCart(int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = await _db.CartDetails.FirstOrDefaultAsync(u => u.CartDetailId == cartDetailsId);
                int totalCountOfCartItem = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();


                _db.CartDetails.Remove(cartDetails);
                if (totalCountOfCartItem == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _db.SaveChangesAsync();

                return true;
            }
            catch(Exception ex)
            {
                return false;

            }

        }
    }
}