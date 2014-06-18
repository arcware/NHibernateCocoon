using System.IO;
using NHibernate.Criterion;
using NHibernateCocoon.Tests.Entities;
using NUnit.Framework;

namespace NHibernateCocoon.Tests
{
	[TestFixture]
	public class NHibernateRepositoryFixture
	{
		private readonly NHibernateRepository<Album> _albumRepo = new NHibernateRepository<Album>();
		private readonly NHibernateRepository<Artist> _artistRepo = new NHibernateRepository<Artist>();
		private readonly NHibernateRepository<Customer> _customerRepo = new NHibernateRepository<Customer>();
		private readonly NHibernateRepository<Employee> _employeeRepo = new NHibernateRepository<Employee>();
		private readonly NHibernateRepository<Genre> _genreRepo = new NHibernateRepository<Genre>();
		private readonly NHibernateRepository<Invoice> _invoiceRepo = new NHibernateRepository<Invoice>();
		private readonly NHibernateRepository<InvoiceLine> _invoiceLineRepo = new NHibernateRepository<InvoiceLine>();
		private readonly NHibernateRepository<MediaType> _mediaTypeRepo = new NHibernateRepository<MediaType>();
		private readonly NHibernateRepository<Playlist> _playlistRepo = new NHibernateRepository<Playlist>();
		private readonly NHibernateRepository<PlaylistTrack> _playlistTrackRepo = new NHibernateRepository<PlaylistTrack>();
		private readonly NHibernateRepository<Track> _trackRepo = new NHibernateRepository<Track>();

		[TestFixtureSetUp]
		public void setup()
		{
			// Use a copy of the database, overwriting if already there
			File.Copy("Chinook.sqlite", "Chinook.working.sqlite", true);
		}

		[TestFixtureTearDown]
		public void teardown()
		{
			// Kill the working db copy
			File.Delete("Chinook.working.sqlite");
		}

		/// <summary>
		/// Get album with ID 81
		/// </summary>
		[Test]
		public void get()
		{
			var album = _albumRepo.Get(81);

			Assert.IsNotNull(album);
			Assert.AreEqual("One By One", album.Title);
		}

		/// <summary>
		/// Get artist with name Foo Fighters
		/// </summary>
		[Test]
		public void get_by_property()
		{
			var artist = _artistRepo.GetByProperty("Name", "Foo Fighters");

			Assert.IsNotNull(artist);
			Assert.AreEqual("Foo Fighters", artist.Name);
		}

		/// <summary>
		/// Get email property for customer with ID 28
		/// </summary>
		[Test]
		public void get_property()
		{
			var property = _customerRepo.GetProperty<string>("Email", "CustomerId", 28);

			Assert.AreEqual("jubarnett@gmail.com", property);
		}

		/// <summary>
		/// Get title, email, and country properties for employee with ID 6
		/// </summary>
		[Test]
		public void get_properties()
		{
			var properties = _employeeRepo.GetProperties(new[] { "Title, Email, Country" }, "EmployeeId", 6);
			var values = (object[])properties[0];

			Assert.IsNotEmpty(properties);
			Assert.AreEqual("IT Manager", values[0]);
			Assert.AreEqual("michael@chinookcorp.com", values[1]);
			Assert.AreEqual("Canada", values[2]);
		}

		/// <summary>
		/// Finds the first genre where the name contains "ck"
		/// </summary>
		[Test]
		public void find_first()
		{
			var criteria = _genreRepo.Session.CreateCriteria<Genre>();
			criteria.Add(Restrictions.Like("Name", "%ck%"));

			var genre = _genreRepo.FindFirst(criteria);

			Assert.IsNotNull(genre);
			Assert.IsTrue(genre.Name.Contains("ck"));
		}

		/// <summary>
		/// Finds the most recent invoice from Germany
		/// </summary>
		[Test]
		public void find_first_with_sorts()
		{
			var criteria = _invoiceRepo.Session.CreateCriteria<Invoice>();
			criteria.Add(Restrictions.Eq("BillingCountry", "Germany"));

			var sortInvoiceDateDesc = new NHibernateRepositorySort("InvoiceDate", NHibernateRepositorySortDirection.Desc);
			var sorts = new NHibernateRepositorySortList { sortInvoiceDateDesc };

			var invoice = _invoiceRepo.FindFirst(criteria, sorts);

			Assert.IsNotNull(invoice);
			Assert.AreEqual("Germany", invoice.BillingCountry);
			Assert.AreEqual(5.94D, invoice.Total);
		}

		/// <summary>
		/// Finds the line items that belong to invoice with ID 3
		/// </summary>
		[Test]
		public void find()
		{
			var criteria = _invoiceLineRepo.Session.CreateCriteria<InvoiceLine>();
			criteria.Add(Restrictions.Eq("InvoiceId", 3));

			var totalItems = 0;
			var list = _invoiceLineRepo.Find(criteria, out totalItems);

			Assert.IsNotEmpty(list);
			Assert.AreEqual(6, totalItems);
		}

		/// <summary>
		/// Finds the line items that belong to invoice with ID 11, sorted by track ID in descending order
		/// </summary>
		[Test]
		public void find_with_sorts()
		{
			var criteria = _invoiceLineRepo.Session.CreateCriteria<InvoiceLine>();
			criteria.Add(Restrictions.Eq("InvoiceId", 11));

			var sortTrackIdDesc = new NHibernateRepositorySort("TrackId", NHibernateRepositorySortDirection.Desc);
			var sorts = new NHibernateRepositorySortList { sortTrackIdDesc };

			var totalItems = 0;
			var list = _invoiceLineRepo.Find(criteria, sorts, out totalItems);

			Assert.IsNotEmpty(list);
			Assert.AreEqual(9, totalItems);
			Assert.AreEqual(322, list[0].TrackId);
		}

		/// <summary>
		/// Finds the first page line items (page size of 5) that belong to invoice with ID 12, sorted by track ID in ascending order
		/// </summary>
		[Test]
		public void find_with_sort_and_paging()
		{
			var criteria = _invoiceLineRepo.Session.CreateCriteria<InvoiceLine>();
			criteria.Add(Restrictions.Eq("InvoiceId", 12));

			var sortTrackIdAsc = new NHibernateRepositorySort("TrackId", NHibernateRepositorySortDirection.Asc);
			var sorts = new NHibernateRepositorySortList { sortTrackIdAsc };

			var totalItems = 0;
			var list = _invoiceLineRepo.Find(criteria, sorts, 0, 5, out totalItems);

			Assert.IsNotEmpty(list);
			Assert.AreEqual(14, totalItems);
			Assert.AreEqual(5, list.Count);
			Assert.AreEqual(331, list[0].TrackId);
			Assert.AreEqual(340, list[1].TrackId);
			Assert.AreEqual(349, list[2].TrackId);
			Assert.AreEqual(358, list[3].TrackId);
			Assert.AreEqual(367, list[4].TrackId);
		}

		/// <summary>
		/// Finds all media types
		/// </summary>
		[Test]
		public void find_all()
		{
			var totalItems = 0;
			var list = _mediaTypeRepo.FindAll(out totalItems);

			Assert.IsNotEmpty(list);
			Assert.AreEqual(5, totalItems);
		}

		/// <summary>
		/// Finds all media types, sorted alphabetically by name
		/// </summary>
		[Test]
		public void find_all_with_sorts()
		{
			var sortNameAsc = new NHibernateRepositorySort("Name", NHibernateRepositorySortDirection.Asc);
			var sorts = new NHibernateRepositorySortList { sortNameAsc };

			var totalItems = 0;
			var list = _mediaTypeRepo.FindAll(sorts, out totalItems);

			Assert.IsNotEmpty(list);
			Assert.AreEqual(5, totalItems);
			Assert.AreEqual("AAC audio file", list[0].Name);
		}

		/// <summary>
		/// Finds the first page of media types (page size of 2), sorted alphabetically by name
		/// </summary>
		[Test]
		public void find_all_with_sorts_and_paging()
		{
			var sortNameAsc = new NHibernateRepositorySort("Name", NHibernateRepositorySortDirection.Asc);
			var sorts = new NHibernateRepositorySortList { sortNameAsc };

			var totalItems = 0;
			var list = _mediaTypeRepo.FindAll(sorts, 0, 2, out totalItems);

			Assert.IsNotEmpty(list);
			Assert.AreEqual(5, totalItems);
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("AAC audio file", list[0].Name);
			Assert.AreEqual("MPEG audio file", list[1].Name);
		}

		/// <summary>
		/// Find tracks where the composer property is Foo Fighters
		/// </summary>
		[Test]
		public void find_by_property()
		{
			var totalItems = 0;
			var list = _trackRepo.FindByProperty("Composer", "Foo Fighters", out totalItems);

			Assert.IsNotEmpty(list);
			Assert.AreEqual(11, totalItems);

			foreach (var track in list)
			{
				Assert.AreEqual("Foo Fighters", track.Composer);
			}
		}

		/// <summary>
		/// Find tracks where the composer property is Foo Fighters, sorted alphabetically by name
		/// </summary>
		[Test]
		public void find_by_property_with_sorts()
		{
			var sortNameAsc = new NHibernateRepositorySort("Name", NHibernateRepositorySortDirection.Asc);
			var sorts = new NHibernateRepositorySortList { sortNameAsc };

			var totalItems = 0;
			var list = _trackRepo.FindByProperty("Composer", "Foo Fighters", sorts, out totalItems);

			Assert.IsNotEmpty(list);
			Assert.AreEqual(11, totalItems);

			foreach (var track in list)
			{
				Assert.AreEqual("Foo Fighters", track.Composer);
			}

			Assert.AreEqual("All My Life", list[0].Name);
		}

		/// <summary>
		/// Finds the first page of tracks (page size of 3) where the composer property is Foo Fighters, sorted alphabetically by name
		/// </summary>
		[Test]
		public void find_by_property_with_sorts_and_paging()
		{
			var sortNameAsc = new NHibernateRepositorySort("Name", NHibernateRepositorySortDirection.Asc);
			var sorts = new NHibernateRepositorySortList { sortNameAsc };

			var totalItems = 0;
			var list = _trackRepo.FindByProperty("Composer", "Foo Fighters", sorts, 0, 3, out totalItems);

			Assert.IsNotEmpty(list);
			Assert.AreEqual(11, totalItems);

			foreach (var track in list)
			{
				Assert.AreEqual("Foo Fighters", track.Composer);
			}

			Assert.AreEqual("All My Life", list[0].Name);
			Assert.AreEqual("Burn Away", list[1].Name);
			Assert.AreEqual("Come Back", list[2].Name);
		}

		/// <summary>
		/// Save, update, and delete a playlist
		/// </summary>
		[Test]
		public void save_update_delete()
		{
			var playlist = new Playlist { Name = "New Playlist" };

			_playlistRepo.Save(playlist);
			Assert.IsTrue(playlist.PlaylistId > 0);

			var playlistId = playlist.PlaylistId;
			playlist = _playlistRepo.Get(playlistId);
			Assert.IsNotNull(playlist);
			Assert.AreEqual(playlistId, playlist.PlaylistId);

			playlist.Name = "Updated Playlist";
			_playlistRepo.Update(playlist);

			playlist = _playlistRepo.Get(playlistId);
			Assert.IsNotNull(playlist);
			Assert.AreEqual("Updated Playlist", playlist.Name);

			_playlistRepo.Delete(playlist);
			playlist = _playlistRepo.Get(playlistId);
			Assert.IsNull(playlist);
		}

		/// <summary>
		/// Counts all tracks in the playlist with ID 8
		/// </summary>
		[Test]
		public void count()
		{
			var criteria = _playlistTrackRepo.Session.CreateCriteria<PlaylistTrack>();
			criteria.Add(Restrictions.Eq("PlaylistId", 8));

			var count = _playlistTrackRepo.Count(criteria);

			Assert.AreEqual(3290, count);
		}

		/// <summary>
		/// Checks if any customers exist from state California and country USA
		/// </summary>
		[Test]
		public void exists()
		{
			var criteria = _customerRepo.Session.CreateCriteria<Customer>();
			criteria.Add(Restrictions.Eq("State", "CA"));
			criteria.Add(Restrictions.Eq("Country", "USA"));

			var exists = _customerRepo.Exists(criteria);

			Assert.IsTrue(exists);
		}
	}
}
