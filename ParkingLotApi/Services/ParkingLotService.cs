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
    }
}
