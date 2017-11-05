using System.Collections.Generic;
using GigHub.Models;

namespace GigHub.Repositories
{
    public interface IGigRepository
    {
        Gig GetGigWithAttendees(int gigId);
        IEnumerable<Gig> GetGigsUserAttending(string userId);
        Gig GetGig(int id);
        IEnumerable<Gig> GetUpcomingGigsByArtist(string artistId);
        void Add(Gig gig);
    }
}