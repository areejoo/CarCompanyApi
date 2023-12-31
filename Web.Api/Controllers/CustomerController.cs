﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using System.Collections.Generic;
using Web.Core;
using Web.Core.Entities;
using Web.Core.Interfaces;
using Web.Api.Dtos.Incomming;
using Web.Api.Dtos.Outcomming;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using web.api.Dtos.Incomming;
using Web.Api.Dtos.Outcomming.CustomerDto;
using Web.Api.Dtos.Incomming.CustomerDto;
using Web.Infrastructure.Services;
using System.Collections;

namespace Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public CustomerController(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<CustomerListDto> GetListAsync([FromQuery] CustomerRequestDto request)
        {
            var query = _customerService.GetCustomersQueryable();
            //filtering
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = FilterCustomer(query, request);
            }
            //get Count
            var count = await query.CountAsync();
            //apply  sorting
            if (!string.IsNullOrEmpty(request.Sort))
            {

                query = SortCustomer(query, request);
            }

            //apply pagination

            query = CreatePagination(query, request);

            //output
            var result = await query.ToListAsync();
            var resultDto = _mapper.Map<List<CustomerDto>>(result);
            CustomerListDto carListDto = new CustomerListDto() { CustomerPaginationList = resultDto, Count = count };


            return carListDto;
        }


        [HttpGet("{id}")]
        public async Task<CustomerDto> GetAsync(Guid id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            var customerDto = _mapper.Map<CustomerDto>(customer);

            return customerDto;
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCustomerDto createCustomerDto)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            CustomerDto customerDto = null;
            IQueryable<Customer> query = null;

            try
            {
                if (createCustomerDto.Name == null)
                {
                    return BadRequest("name of customer is required");

                }
                query = _customerService.GetCustomersQueryable();
                if (createCustomerDto.Email != null)
                {
                    query = query.Where(c => c.Email == createCustomerDto.Email);
                }

                if (query.Count() == 0)
                {
                    var customerEntity = _mapper.Map<Customer>(createCustomerDto);
                    var result1 = await _customerService.AddCustomerAsync(customerEntity);
                    if (result1)
                        customerDto = _mapper.Map<CustomerDto>(customerEntity);
                }
                else
                {
                    return BadRequest("email is must be unique");
                }

            }
            catch (Exception e)
            {

            }
            return Ok("done"); //customerDto


        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateCustomerDto updateCustomerDto)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }


            if (updateCustomerDto.Id == null || updateCustomerDto.Id == Guid.Empty)
            {
                return BadRequest(" id is required");
            }
            //var carExists=await _carService.GetCarByIdAsync(updateCarDto.Id);
            CustomerDto customerDto = null;
            IQueryable<Customer> query = null;
            Customer customer = null;
            bool result1 = false;

            try
            {
                customer = await _customerService.GetCustomerByIdAsync(updateCustomerDto.Id);
                if (customer != null)
                {
                    customer.Address = updateCustomerDto.Address!=null? updateCustomerDto.Address:customer.Address;
                    customer.Name = updateCustomerDto.Name!=null?updateCustomerDto.Name:customer.Name ;
                    customer.Phone = updateCustomerDto.Phone != null ? updateCustomerDto.Phone : customer.Phone;
                    if (updateCustomerDto.Email != null)
                    {
                        query = _customerService.GetCustomersQueryable();
                        query = query.Where(c => c.Email == updateCustomerDto.Email);
                        if (query.Count() != 0)
                        {
                            return BadRequest("email is reserved to another customer");

                        }
                        else {
                            customer.Email = updateCustomerDto.Email;
                        }


                    }

                    result1 = await _customerService.UpdateCustomerAsync(customer);
                    if (result1)
                    {
                        customerDto = _mapper.Map<CustomerDto>(customer);
                    }
                }
                else
                {
                    return BadRequest(" customer is not found ");
                }


            }
            catch (Exception ev)
            {
                //_logger.LogInformation(e.ToString());
            }



            return Ok("updated successfuly");
        }






        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = false;
            try
            {
                result = await _customerService.DeleteCustomerAsync(id);

            }

            catch (Exception e)
            {

                return BadRequest();
            }
            if (result)
                return Ok();
            else
                return NotFound();


        }
        private IQueryable<Customer> CreatePagination(IQueryable<Customer> query, CustomerRequestDto request)
        {
            query = query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);
            return query;
        }

        private IQueryable<Customer> SortCustomer(IQueryable<Customer> query, CustomerRequestDto request)
        {

            switch (request.Sort)
            {
                case "Name_desc":
                    query = query.OrderByDescending(c => c.Name);
                    break;
                case "Email_desc":
                    query = query.OrderByDescending(c => c.Email);
                    break;
                case "Email_asc":
                    query = query.OrderBy(c => c.Email);
                    break;
                case "Address_desc":
                    query = query.OrderByDescending(c => c.Address);
                    break;
                case "Address_asc":
                    query = query.OrderBy(c => c.Address);
                    break;
                case "Phone_desc":
                    query = query.OrderByDescending(c => c.Phone);
                    break;
                case "Phone_asc":
                    query = query.OrderBy(c => c.Phone);
                    break;

                default:
                    query = query.OrderBy(c => c.Name);
                    break;

            }
            return query;
        }

        private IQueryable<Customer> FilterCustomer(IQueryable<Customer> query, CustomerRequestDto request)
        {
            query = query.Where(c => c.Name.Trim().Equals(request.Search)
                                           || c.Email.Trim().Equals(request.Search)
                                           || c.Phone.Trim().Equals(request.Search)
                                           || c.Address.Trim().Equals(request.Search));

            return query;
        }

    }


}
