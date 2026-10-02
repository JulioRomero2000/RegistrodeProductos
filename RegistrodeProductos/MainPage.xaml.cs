using RegistrodeProductos.Models;
using RegistrodeProductos.Data;
using System.Globalization;

namespace RegistrodeProductos;

public partial class MainPage : ContentPage
{
    private DatabaseService database;
    public MainPage()
    {
        InitializeComponent();
        database = new DatabaseService();
        FechaPicker.Date = DateTime.Today;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarProximoIdAsync();
    }
    private async Task CargarProximoIdAsync()
    {
        try
        {
            int proximoId = await database.ObtenerProximoIdAsync();
            IdEntry.Text = proximoId.ToString();
        }
        catch
        {
            IdEntry.Text = "Auto";
        }
    }
    private async void GuardarButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NombreEntry.Text))
        {
            await DisplayAlertAsync(
                "Error",
                "Ingrese el nombre del producto.",
                "Aceptar");

            return;
        }

        if (string.IsNullOrWhiteSpace(DescripcionEntry.Text))
        {
            await DisplayAlertAsync(
                "Error",
                "Ingrese la descripción.",
                "Aceptar");

            return;
        }

        if (!double.TryParse(
            PrecioEntry.Text,
            NumberStyles.Number,
            CultureInfo.CurrentCulture,
            out double precio))
        {
            if (!double.TryParse(
                PrecioEntry.Text,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out precio))
            {
                await DisplayAlertAsync(
                    "Error",
                    "Ingrese un precio válido.",
                    "Aceptar");

                return;
            }
        }

        if (precio <= 0)
        {
            await DisplayAlertAsync(
                "Error",
                "El precio debe ser mayor que cero.",
                "Aceptar");

            return;
        }

        if (!int.TryParse(CantidadEntry.Text, out int cantidad))
        {
            await DisplayAlertAsync(
                "Error",
                "Ingrese una cantidad válida.",
                "Aceptar");

            return;
        }

        if (cantidad <= 0)
        {
            await DisplayAlertAsync(
                "Error",
                "La cantidad debe ser mayor que cero.",
                "Aceptar");

            return;
        }

        try
        {
            Producto producto = new Producto
            {
                Nombre = NombreEntry.Text.Trim(),
                Descripcion = DescripcionEntry.Text.Trim(),
                Precio = precio,
                Cantidad = cantidad,
                FechaRegistro = FechaPicker.Date ?? DateTime.Today
            };

            GuardarButton.IsEnabled = false;

            int resultado = await database.GuardarProductoAsync(producto);

            if (resultado > 0)
            {
                await DisplayAlertAsync(
                    "Registro exitoso",
                    "Producto guardado correctamente.\nID asignado: " + producto.ID,
                    "Aceptar");

                NombreEntry.Text = "";
                DescripcionEntry.Text = "";
                PrecioEntry.Text = "";
                CantidadEntry.Text = "";
                FechaPicker.Date = DateTime.Today;

                await CargarProximoIdAsync();
            }
            else
            {
                await DisplayAlertAsync(
                    "Error",
                    "No se pudo guardar el producto.",
                    "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Error",
                ex.Message,
                "Aceptar");
        }
        finally
        {
            GuardarButton.IsEnabled = true;
        }
    }
    private async void MostrarButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            List<Producto> productos =
                await database.ObtenerProductosAsync();

            if (productos.Count == 0)
            {
                await DisplayAlertAsync(
                    "Productos",
                    "No hay productos registrados.",
                    "Aceptar");

                return;
            }

            string mensaje = "";

            foreach (Producto producto in productos)
            {
                mensaje += "ID: " + producto.ID + "\n";
                mensaje += "Nombre: " + producto.Nombre + "\n";
                mensaje += "Descripción: " + producto.Descripcion + "\n";
                mensaje += "Precio: " + producto.Precio.ToString("F2") + "\n";
                mensaje += "Cantidad: " + producto.Cantidad + "\n";
                mensaje += "Fecha: " + producto.FechaRegistro.ToString("dd/MM/yyyy");
                mensaje += "\n-------------------------\n";
            }

            await DisplayAlertAsync(
                "Productos registrados",
                mensaje,
                "Cerrar");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Error",
                ex.Message,
                "Aceptar");
        }
    }
}