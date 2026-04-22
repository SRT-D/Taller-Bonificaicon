using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgramacionFuncionalVehiculos
{
    public record Producto(int Id, string Nombre, string Categoria, decimal Precio, int Stock, bool Activo);

    class Program
    {
        static void Main(string[] args)
        {
            var inventario = new List<Producto>
            {
                new Producto(1, "Spoiler de Carbono", "Estetica", 3396000m, 5, true),
                new Producto(2, "Volante Fibra de Carbono Serie G", "Estetica", 3599000m, 3, true),
                new Producto(3, "Espejos en Carbono", "Estetica", 1590000m, 10, true),
                new Producto(4, "Faldones Laterales en Fibra de Carbono BMW", "Estetica", 8000000m, 4, true),
                new Producto(5, "Frenos Carboceramicos", "Rendimiento", 17957764m, 2, true),
                new Producto(6, "Suspension Roscada", "Rendimiento", 6000000m, 4, true),
                new Producto(7, "Downpipe", "Rendimiento", 3000000m, 1, true),
                new Producto(8, "Filtro de alto flujo", "Rendimiento", 400000m, 8, true),
                new Producto(9, "Intercooler de Alto Flujo", "Rendimiento", 2000000m, 0, true)
            };

            List<Producto> carrito = new List<Producto>();
            bool salir = false;

           
            static Producto AplicarDescuento(decimal porcentaje, Producto producto) => producto with { Precio = producto.Precio * (1 - porcentaje / 100) };
            static Producto AplicarIVA(decimal tasa, Producto producto) => producto with { Precio = producto.Precio * (1 + tasa / 100) };
            static Producto AjustarStock(int cantidad, Producto producto) => producto with { Stock = producto.Stock - cantidad };

            
            static Producto GenerarVenta(decimal porcentajeDescuento, decimal tasaIva, int cantidad, Producto producto) =>
                AjustarStock(cantidad, AplicarIVA(tasaIva, AplicarDescuento(porcentajeDescuento, producto)));

            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("=== PERFORMANCE SHOP - MENU PRINCIPAL ===");
                Console.WriteLine("1. Ir a Comprar ");
                Console.WriteLine("2. Reporte: Productos Mas Caros");
                Console.WriteLine("3. Salir");
                

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        bool comprando = true;
                        while (comprando)
                        {
                            var categorias = inventario.Select(producto => producto.Categoria).Distinct().ToList();
                            Console.WriteLine("\n--- SELECCIONE CATEGORIA ---");
                            for (int i = 0; i < categorias.Count; i++) Console.WriteLine((i + 1) + ". " + categorias[i]);

                            if (int.TryParse(Console.ReadLine(), out int seleccionUsuario) && seleccionUsuario <= categorias.Count)
                            {
                                string catSeleccionada = categorias[seleccionUsuario - 1];
                                var filtrados = inventario.Where(producto => producto.Categoria == catSeleccionada && producto.Stock > 0).ToList();

                                Console.WriteLine("\n--- PRODUCTOS EN " + catSeleccionada.ToUpper() + " ---");
                                filtrados.ForEach(producto => Console.WriteLine("ID: " + producto.Id + " | " + producto.Nombre + " | Stock: " + producto.Stock + " | Precio: " + producto.Precio.ToString("C")));

                                Console.Write("\nIngrese ID para añadir al carrito (0 para cancelar): ");
                                if (int.TryParse(Console.ReadLine(), out int idSeleccionado) && idSeleccionado != 0)
                                {
                                    var encontrado = inventario.FirstOrDefault(producto => producto.Id == idSeleccionado && producto.Stock > 0);
                                    if (encontrado != null)
                                    {
                                        carrito.Add(encontrado);
                                        inventario = inventario.Select(producto => producto.Id == idSeleccionado ? AjustarStock(1, producto) : producto).ToList();
                                        Console.WriteLine("\n" + encontrado.Nombre + " añadido al carrito.");
                                    }
                                    else Console.WriteLine("Producto no disponible");
                                }
                            }

                            Console.WriteLine("\n1. Añadir otro objeto | 2. Pagar y Facturar");
                            if (Console.ReadLine() == "2")
                            {
                                FinalizarCompra(carrito, GenerarVenta);
                                carrito.Clear();
                                comprando = false;
                            }
                        }
                        break;

                    case "2":
                        MostrarReporteCaros(inventario);
                        Console.WriteLine("\nPresione cualquier tecla para volver al menu principal.");
                        Console.ReadKey();
                        break;

                    case "3":
                        salir = true;
                        break;
                }
            }
        }

        static void FinalizarCompra(List<Producto> carrito, Func<decimal, decimal, int, Producto, Producto> generarVenta)
        {
            if (!carrito.Any()) return;

            decimal subtotalBase = 0;
            decimal totalConDescuento = 0;
            decimal totalIva = 0;
            decimal totalFinal = 0;

            Console.WriteLine("\n========================================");
            Console.WriteLine("           FACTURA DE VENTA");
            Console.WriteLine("========================================");

            for (int i = 0; i < carrito.Count; i++)
            {
                var productoBase = carrito[i];
                
                var productoFinal = generarVenta(5, 19, 1, productoBase);

                decimal descuentoIndividual = productoBase.Precio * 0.05m;
                decimal ivaIndividual = (productoBase.Precio - descuentoIndividual) * 0.19m;

                subtotalBase += productoBase.Precio;
                totalConDescuento += (productoBase.Precio - descuentoIndividual);
                totalIva += ivaIndividual;
                totalFinal += productoFinal.Precio;

                Console.WriteLine("- " + productoBase.Nombre);
                Console.WriteLine("  Precio Base: " + productoBase.Precio.ToString("C"));
            }

            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Subtotal: " + subtotalBase.ToString("C"));
            Console.WriteLine("Descuento (5%): -" + (subtotalBase * 0.05m).ToString("C"));
            Console.WriteLine("IVA (19%): +" + totalIva.ToString("C"));
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("TOTAL A PAGAR: " + totalFinal.ToString("C"));
            Console.WriteLine("========================================");
            Console.WriteLine("\nPresione una tecla para continuar");
            Console.ReadKey();
        }

        static void MostrarReporteCaros(List<Producto> inventario)
        {
            var carosPorCat = inventario
                .GroupBy(producto => producto.Categoria) .Select(categoria => categoria.OrderByDescending(p => p.Precio).FirstOrDefault())
                .ToList();

            Console.WriteLine("\n--- Producto Mas Caro ---");
           

            for (int i = 0; i < carosPorCat.Count; i++)
            {
                var producto = carosPorCat[i];
                if (producto != null)
                {
                    Console.WriteLine("- En " + producto.Categoria + ": " + producto.Nombre + " (" + producto.Precio.ToString("C") + ")");
                }
            }
        }
    }
}
