using RecetarioMaui.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows.Input;

namespace RecetarioMaui.ViewModels
{
    public class RecetarioViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<RecetaModel> Recetas { get; set; } = new();
        public RecetaModel Receta { get; set; } = new();

        public string Errores { get; set; } = "";
        private int indiceRecetaEditar;

        public ICommand VerRecetarioCommand { get; set; }
        public ICommand VerAgregarCommand { get; set; }
        public ICommand VerEditarCommand { get; set; }
        public ICommand VerEliminarCommand { get; set; }
        public ICommand VerRecetaInformacionCommand { get; set; }
        public ICommand AgregarCommand { get; set; }
        public ICommand EditarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }
        public ICommand PrepararRecetaCommand { get; set; }
        public RecetarioViewModel()
        {
            VerRecetarioCommand = new Command(VerRecetario);
            VerAgregarCommand = new Command(VerAgregar);
            VerEditarCommand = new Command(VerEditar);
            VerEliminarCommand = new Command(VerEliminar);
            VerRecetaInformacionCommand = new Command<RecetaModel>(VerRecetaInformacion);

            AgregarCommand = new Command(Agregar);
            EditarCommand = new Command(Editar);
            EliminarCommand = new Command(Eliminar);

            PrepararRecetaCommand = new Command(PrepararReceta);

            Cargar();
        }

        private void PrepararReceta()
        {
            Recetas[indiceRecetaEditar].UltimaElaboracion = DateTime.Now;
            Receta.UltimaElaboracion = DateTime.Now;
            PropertyChanged?.Invoke(this, new(nameof(Recetas)));
            PropertyChanged?.Invoke(this, new(nameof(Receta)));
            Editar();
        }
        private void Eliminar()
        {
            if (Receta != null)
            {
                Recetas.RemoveAt(indiceRecetaEditar);
                Guardar();
                PropertyChanged?.Invoke(this, new(nameof(Recetas)));
                VerRecetario();
            }
        }
        private void Editar()
        {
            Errores = "";
            if (Receta != null)
            {
                if (string.IsNullOrEmpty(Receta.Nombre))
                {
                    Errores += "Escribe el nombre de la receta\n";
                }
                if (string.IsNullOrEmpty(Receta.Procedimineto))
                {
                    Errores += "Escribe el procedimiento de la receta\n";
                }
                if (string.IsNullOrEmpty(Receta.Ingredientes))
                {
                    Errores += "Escribe los ingredientes de la receta\n";
                }
                if (Receta.TiempoPreparacion <= 0)
                {
                    Errores += "Pon un tiempo de preparacion mayor a 0\n";
                }
                if (Recetas.Any(x => Recetas[indiceRecetaEditar] != x && x.Nombre == Receta.Nombre))
                {
                    Errores += "El nombre de la receta no se puede repetir\n";
                }
                PropertyChanged?.Invoke(this, new(nameof(Errores)));
                if (string.IsNullOrEmpty(Errores))
                {
                    Recetas.RemoveAt(indiceRecetaEditar);
                    Recetas.Add(Receta);
                    Guardar();
                    Receta = new();
                    PropertyChanged?.Invoke(this, new(nameof(Receta)));
                    PropertyChanged?.Invoke(this, new(nameof(Errores)));
                    PropertyChanged?.Invoke(this, new(nameof(Recetas)));
                    VerRecetario();
                }
            }
        }
        private void Agregar()
        {
            Errores = "";
            if (Receta != null)
            {
                if (string.IsNullOrEmpty(Receta.Nombre))
                {
                    Errores += "Escribe el nombre de la receta\n";
                }
                if (string.IsNullOrEmpty(Receta.Procedimineto))
                {
                    Errores += "Escribe el procedimiento de la receta\n";
                }
                if (string.IsNullOrEmpty(Receta.Ingredientes))
                {
                    Errores += "Escribe los ingredientes de la receta\n";
                }
                if (Receta.TiempoPreparacion <= 0)
                {
                    Errores += "Pon un tiempo de preparacion mayor a 0\n";
                }
                PropertyChanged?.Invoke(this, new(nameof(Errores)));
                if (string.IsNullOrEmpty(Errores))
                {
                    Recetas.Add(Receta);
                    Guardar();
                    Receta = new();
                    PropertyChanged?.Invoke(this, new(nameof(Receta)));
                    PropertyChanged?.Invoke(this, new(nameof(Recetas)));
                    PropertyChanged?.Invoke(this, new(nameof(Errores)));
                    VerRecetario();
                }
            }
        }

        private void VerRecetaInformacion(RecetaModel r)
        {
            var clon = new RecetaModel
            {
                Imagen = r.Imagen,
                Ingredientes = r.Ingredientes,
                Nombre = r.Nombre,
                Procedimineto = r.Procedimineto,
                TiempoPreparacion = r.TiempoPreparacion,
                UltimaElaboracion = r.UltimaElaboracion
            };
            Receta = clon;
            indiceRecetaEditar = Recetas.IndexOf(r);
            Errores = "";
            PropertyChanged?.Invoke(this, new(nameof(Errores)));
            PropertyChanged?.Invoke(this, new(nameof(Receta)));
            Shell.Current.GoToAsync("//Informacion");
        }
        private void VerEliminar()
        {
            Shell.Current.GoToAsync("//Eliminar");
        }
        private void VerEditar()
        {
            PropertyChanged?.Invoke(this, new(nameof(Errores)));
            PropertyChanged?.Invoke(this, new(nameof(Receta)));
            Shell.Current.GoToAsync("//Editar");
        }
        private void VerAgregar()
        {
            Receta = new();
            PropertyChanged?.Invoke(this, new(nameof(Errores)));
            PropertyChanged?.Invoke(this, new(nameof(Receta)));
            Shell.Current.GoToAsync("//Agregar");
        }
        private void VerRecetario()
        {
            Shell.Current.GoToAsync("//ListaRecetas");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Guardar()
        {
            Recetas = new ObservableCollection<RecetaModel>(Recetas.OrderByDescending(r => r.UltimaElaboracion));
            var ruta = FileSystem.AppDataDirectory;
            File.WriteAllText(ruta + "/Receta.json", JsonSerializer.Serialize(Recetas));
        }
        private void Cargar()
        {
            var ruta = FileSystem.AppDataDirectory;
            if (File.Exists(ruta + "/Receta.json"))
            {
                var datos = JsonSerializer.Deserialize<ObservableCollection<RecetaModel>>(File.ReadAllText(ruta + "/Receta.json"));
                if (datos != null)
                {
                    foreach (var receta in datos)
                    {
                        Recetas.Add(receta);
                    }
                }
            }
        }
    }
}