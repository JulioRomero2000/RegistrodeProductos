using SQLite;
using RegistrodeProductos.Models;

namespace RegistrodeProductos.Data
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection database;

        public DatabaseService()
        {
            string ruta = Path.Combine(
                FileSystem.AppDataDirectory,
                "productos.db3"
            );

            database = new SQLiteAsyncConnection(ruta);
        }
        public async Task InicializarAsync()
        {
            await database.CreateTableAsync<Producto>();
        }
        public async Task<int> ObtenerProximoIdAsync()
        {
            await InicializarAsync();
            var ultimo = await database.Table<Producto>().OrderByDescending(p => p.ID).FirstOrDefaultAsync();
            return ultimo != null ? ultimo.ID + 1 : 1;
        }
        public async Task<int> GuardarProductoAsync(Producto producto)
        {
            await InicializarAsync();

            return await database.InsertAsync(producto);
        }
        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            await InicializarAsync();

            return await database.Table<Producto>().ToListAsync();
        }
    }
}