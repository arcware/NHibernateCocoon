namespace NHibernateCocoon.Tests.Entities
{
	public class Album
	{
		public virtual int AlbumId { get; set; }
		public virtual string Title { get; set; }
		public virtual int ArtistId { get; set; }
	}
}
