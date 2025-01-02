using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.IO;
using MongoDB.Bson;

namespace MediaPlayerWinUI
{
    class FileService
    {
        private readonly IMongoCollection<File> _filesCollection;
        private readonly string _storagePath = "C:\\MediaFiles"; // Đường dẫn lưu file trên hệ thống

        public FileService()
        {
            var client = new MongoClient("mongodb://localhost:27017/mediaplayer");
            var database = client.GetDatabase("MediaPlayer");
            _filesCollection = database.GetCollection<File>("files");

            // Tạo thư mục lưu trữ nếu chưa tồn tại
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task<string> AddFileAsync(
            string filePath,
            string role,
            string artist,
            string title,
            string albumId,
            string playlistId,
            string genre,
            TimeSpan runtime,
            string thumbnailPath = null)
        {
            try
            {
                var fileName = Path.GetFileName(filePath);
                var newFilePath = Path.Combine(_storagePath, fileName);
                System.IO.File.Copy(filePath, newFilePath, overwrite: true);

                string thumbnailUrl = null;
                if (thumbnailPath != null)
                {
                    var thumbnailFileName = Path.GetFileName(thumbnailPath);
                    thumbnailUrl = Path.Combine(_storagePath, thumbnailFileName);
                    System.IO.File.Copy(thumbnailPath, thumbnailUrl, overwrite: true); 
                }

                var fileMetadata = new File
                {
                    Role = role,
                    Url = newFilePath,
                    Artist = artist,
                    Title = title,
                    AlbumId = albumId,
                    PlaylistId = playlistId,
                    Thumbnail = thumbnailUrl ?? "default_thumbnail.png",
                    Runtime = runtime,
                    Genre = genre,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _filesCollection.InsertOneAsync(fileMetadata);
                return fileMetadata.Id.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding file: {ex.Message}");
            }
        }

        public async Task<List<File>> GetAllFilesAsync()
        {
            try
            {
                // Truy vấn tất cả các file trong cơ sở dữ liệu
                var files = await _filesCollection.Find(Builders<File>.Filter.Empty).ToListAsync();
                return files;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting all files: {ex.Message}");
            }
        }

        public async Task<List<File>> GetFilesByAlbumIdAsync(string albumId)
        {
            try
            {
                // Truy vấn các file có albumId tương ứng
                var filter = Builders<File>.Filter.Eq(f => f.AlbumId, albumId);
                var files = await _filesCollection.Find(filter).ToListAsync();
                return files;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting files by albumId: {ex.Message}");
            }
        }

        public async Task<List<File>> GetFilesByPlaylistIdAsync(string playlistId)
        {
            try
            {
                // Truy vấn các file có playlistId tương ứng
                var filter = Builders<File>.Filter.Eq(f => f.PlaylistId, playlistId);
                var files = await _filesCollection.Find(filter).ToListAsync();
                return files;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting files by playlistId: {ex.Message}");
            }
        }

        public async Task<bool> UpdateFileAsync(string fileId, string role, string artist, string title, string albumId, string playlistId, string genre, TimeSpan runtime, string thumbnailPath = null)
        {
            try
            {
                // Tìm kiếm file bằng Id
                var filter = Builders<File>.Filter.Eq(f => f.Id, new ObjectId(fileId));
                var fileToUpdate = await _filesCollection.Find(filter).FirstOrDefaultAsync();
                if (fileToUpdate == null)
                {
                    throw new Exception("File not found.");
                }

                // Cập nhật thông tin metadata
                fileToUpdate.Role = role;
                fileToUpdate.Artist = artist;
                fileToUpdate.Title = title;
                fileToUpdate.AlbumId = albumId;
                fileToUpdate.PlaylistId = playlistId;
                fileToUpdate.Genre = genre;
                fileToUpdate.Runtime = runtime;
                fileToUpdate.UpdatedAt = DateTime.Now;

                // Cập nhật thumbnail nếu có
                if (!string.IsNullOrEmpty(thumbnailPath))
                {
                    var thumbnailFileName = Path.GetFileName(thumbnailPath);
                    fileToUpdate.Thumbnail = Path.Combine(_storagePath, thumbnailFileName);
                    System.IO.File.Copy(thumbnailPath, fileToUpdate.Thumbnail, overwrite: true);
                }

                // Cập nhật thông tin vào MongoDB
                var update = Builders<File>.Update
                    .Set(f => f.Role, fileToUpdate.Role)
                    .Set(f => f.Artist, fileToUpdate.Artist)
                    .Set(f => f.Title, fileToUpdate.Title)
                    .Set(f => f.AlbumId, fileToUpdate.AlbumId)
                    .Set(f => f.PlaylistId, fileToUpdate.PlaylistId)
                    .Set(f => f.Genre, fileToUpdate.Genre)
                    .Set(f => f.Runtime, fileToUpdate.Runtime)
                    .Set(f => f.UpdatedAt, fileToUpdate.UpdatedAt)
                    .Set(f => f.Thumbnail, fileToUpdate.Thumbnail);

                var result = await _filesCollection.UpdateOneAsync(filter, update);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating file: {ex.Message}");
            }
        }

        public async Task<bool> DeleteFileAsync(string fileId)
        {
            try
            {
                // Tìm kiếm file theo Id
                var filter = Builders<File>.Filter.Eq(f => f.Id, new ObjectId(fileId));
                var result = await _filesCollection.DeleteOneAsync(filter);

                // Nếu xóa thành công, trả về true
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting file: {ex.Message}");
            }
        }

    }
}
