﻿using Microsoft.EntityFrameworkCore;
using ParkingLotApi.Dtos;
using ParkingLotApi.Model;
using ParkingLotApi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingLotApi.Services
{
    public class ParkingLotService
    {
        private ParkingLotContext parkingLotContext;
        private int pageSize = 15;

        public ParkingLotService(ParkingLotContext parkingLotContext)
        {
            this.parkingLotContext = parkingLotContext;
        }
        public async Task<int> AddParkingLot(ParkingLotDto parkingLotDto)
        {
            if(parkingLotContext.ParkingLots.FirstOrDefault(parkinglot => parkinglot.Name == parkingLotDto.Name) != null)
            {
                return -1;
            }
            ParkingLotEntity parkingLotEntity = parkingLotDto.ToEntity();
            await parkingLotContext.ParkingLots.AddAsync(parkingLotEntity);
            await parkingLotContext.SaveChangesAsync();
            return parkingLotEntity.Id;
        }

        public async Task<List<ParkingLotDto>> GetAllParkingLots()
        {
            var parkingLots = parkingLotContext.ParkingLots;
            return parkingLots.Select(parkingLotEntity => new ParkingLotDto(parkingLotEntity)).ToList();
        }

        public async Task<ParkingLotDto> GetParkingLotById(int id)
        {
            var foundParkingLot = await parkingLotContext.ParkingLots.FirstOrDefaultAsync(parkingLot => parkingLot.Id == id);
            return new ParkingLotDto(foundParkingLot);
        }

        public async Task DeleteParkingLotById(int id)
        {
            var foundParkingLot = parkingLotContext.ParkingLots.FirstOrDefault(parkingLot => parkingLot.Id == id);
            parkingLotContext.ParkingLots.Remove(foundParkingLot);
            await parkingLotContext.SaveChangesAsync();
        }

        public async Task<List<ParkingLotDto>> GetByPage(int startPage)
        {
            var parkingLots = await parkingLotContext.ParkingLots.ToListAsync();
            var selectedPage = parkingLots.Select(parkingLot => parkingLot).Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
            return selectedPage.Select(parkingLot => new ParkingLotDto(parkingLot)).ToList();
        }

        public async Task<ParkingLotDto> UpdateParkingLotById(ParkingLotDto parkingLotDto,int id)
        {
            var foundParkingLot = parkingLotContext.ParkingLots.FirstOrDefault(parkingLot => parkingLot.Id == id);
            foundParkingLot.Capcity = parkingLotDto.Capcity;
            await parkingLotContext.SaveChangesAsync();
            return new ParkingLotDto(foundParkingLot);
        }

        public async Task<int> AddOrder(OrderDto orderDto,int id)
        {
            OrderEntity orderEntity = orderDto.ToEntity();
            var parkingLots= await parkingLotContext.ParkingLots.
                Include(parklot => parklot.Orders).ToListAsync();
            var foundParkingLot = parkingLots.FirstOrDefault(parkingLot => parkingLot.Id == id);
            var openCount = foundParkingLot.Orders?.Where(order => order.OrderStatus == "open").Count();
            if(foundParkingLot.Capcity == openCount)
            {
                return -1;
            }
            if(foundParkingLot.Orders == null)
            {
                foundParkingLot.Orders = new List<OrderEntity>();
            }
            foundParkingLot.Orders.Add(orderEntity);
            await parkingLotContext.Orders.AddAsync(orderEntity);
            await parkingLotContext.SaveChangesAsync();
            return orderEntity.Id;
        }

        public async Task<List<OrderDto>> GetOrder(int id)
        {
            var parkingLots = await parkingLotContext.ParkingLots.
                Include(parklot => parklot.Orders).ToListAsync();
            var foundParkingLot = parkingLots.FirstOrDefault(parkingLot => parkingLot.Id == id);
            return foundParkingLot.Orders.Select(orderEntity => new OrderDto(orderEntity)).ToList();
        }

        public async Task<OrderDto> UpdateOrder(OrderDto orderDto, int id, string plateNumber)
        {
            var parkingLots = await parkingLotContext.ParkingLots.
                Include(parklot => parklot.Orders).ToListAsync();
            var foundParkingLot = parkingLots.FirstOrDefault(parkingLot => parkingLot.Id == id);
            var foundOrder = foundParkingLot?.Orders.FirstOrDefault(order => order.PlateNumber == plateNumber);
            foundOrder.ClosedTime = orderDto.ClosedTime;
            foundOrder.OrderStatus=orderDto.OrderStatus;
            await parkingLotContext.SaveChangesAsync();
            return orderDto;
        }
    }

}
