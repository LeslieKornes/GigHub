using GigHub.Models;
using GigHub.Repositories;

namespace GigHub.Persistence
{
    public class UnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IGigRepository Gigs { get; private set; }
        public IGenreRepository Genres { get; private set; }
        public FollowingRepository Followings { get; private set; }
        public AttendanceRepository Attendances { get; set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Gigs = new GigRepository(_context);
            Genres = new GenreRepository(_context);
            Followings = new FollowingRepository(_context);
            Attendances = new AttendanceRepository(_context);
        }

        public void Complete()
        {
            _context.SaveChanges();
        }
    }
}