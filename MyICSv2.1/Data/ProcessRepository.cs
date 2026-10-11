using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using MyICSv2._1.Models;

namespace MyICSv2._1.Data
{
    internal class ProcessRepository
    {
        private readonly AppDbContext _context;
        public ProcessRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<IndustryProcess>> GetProcessesAsync()
        {
            return await _context.Processes.OrderBy(p => p.ProcessId).ToListAsync();
        }
    }
}
