using TicketBookingSystem.Models.DTOs;
using TicketBookingSystem.Data;
using TicketBookingSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace TicketBookingSystem.Services
{
    public class StationService : IStationService
    {
        private readonly AppDbContext _context;

        public StationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StationDto>> GetAllAsync()
        {
            return await _context.Stations
                .Select(s => new StationDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    City = s.City,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude
                }).ToListAsync();
        }

        public async Task<StationDto?> GetByIdAsync(int id)
        {
            return await _context.Stations
                .Where(s => s.Id == id)
                .Select(s => new StationDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    City = s.City,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude
                }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<StationDto>> SearchByNameAsync(string name)
        {
            return await _context.Stations
                .Where(s => EF.Functions.ILike(s.Name, $"%{name}%")) // нечутливий до регістру пошук
                .Select(s => new StationDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    City = s.City,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude
                })
                .ToListAsync();
        }


        public async Task<int> CreateAsync(CreateUpdateStationDto dto)
        {
            var station = new Station
            {
                Name = dto.Name,
                City = dto.City,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude
            };

            _context.Stations.Add(station);
            await _context.SaveChangesAsync();
            return station.Id;
        }

        public async Task<bool> UpdateAsync(int id, CreateUpdateStationDto dto)
        {
            var station = await _context.Stations.FindAsync(id);
            if (station == null) return false;

            station.Name = dto.Name;
            station.City = dto.City;
            station.Latitude = dto.Latitude;
            station.Longitude = dto.Longitude;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var station = await _context.Stations.FindAsync(id);
            if (station == null) return false;

            _context.Stations.Remove(station);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}