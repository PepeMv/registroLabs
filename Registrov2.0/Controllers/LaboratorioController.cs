using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Registrov2._0.Models;
using Registrov2._0.Models.ViewModels;
using System.Net;
using System.Dynamic;
using Rotativa;
using Rotativa.Options;
using System.Web.UI.WebControls.WebParts;

namespace Registrov2._0.Controllers
{
    public class LaboratorioController : Controller
    {

        // GET: Laboratorio
        [AllowAnonymous]
        public ActionResult Index()
        {
            LaboratorioSimple lab = new LaboratorioSimple();

            //obtenro la ip y la subred para comparar con la base
            string host = Dns.GetHostName();
            IPAddress[] ips = Dns.GetHostAddresses(host);
            int ultimoValor = ips.Count();
            string myIP = ips[ultimoValor - 1].ToString();
            string[] resultado = myIP.Split('.');
            int mySubred = Convert.ToInt32(resultado[2]);


            using (registroLabsEntities db = new registroLabsEntities())
            {
                lab = (from d in db.laboratorio
                       where d.subred == mySubred
                       select new LaboratorioSimple
                       {
                           Id = d.id,
                           Nombre = d.nombre,
                           Capacidad = d.capacidad.HasValue ? d.capacidad.Value : 0,
                           Subred = d.subred.HasValue ? d.subred.Value : 0
                       }).FirstOrDefault();
            }


            //declaro el modelo que voy a regresar a la vista
            UnionModeloLaboratorioIndex modelo = new UnionModeloLaboratorioIndex();
            modelo.Laboratorio = lab;

            return View(modelo);
        }

        
        public ActionResult LaboratoriosGeneral()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                lst = (from d in db.laboratorio
                       select new SelectListItem
                       {
                           Value = d.id.ToString(),
                           Text = d.nombre,
                       }).ToList();
            }


            List<SelectListItem> listaMaterias = new List<SelectListItem>();
            using (registroLabsEntities db = new registroLabsEntities())
            {
                listaMaterias = (from d in db.materia
                                 select new SelectListItem
                                 {
                                     Value = d.id.ToString(),
                                     Text = d.nombre,
                                 }).ToList();
            }

            List<SelectListItem> listaDocentes = new List<SelectListItem>();
            using (registroLabsEntities db = new registroLabsEntities())
            {
                listaDocentes = (from d in db.docente
                                 select new SelectListItem
                                 {
                                     Value = d.id.ToString(),
                                     Text = d.apellido + " " + d.nombre
                                 }).ToList();
            }


            List<SelectListItem> listaPeriodos = new List<SelectListItem>();
            using (registroLabsEntities db = new registroLabsEntities())
            {
                listaPeriodos = (from d in db.periodo
                                 select new SelectListItem
                                 {
                                     Value = d.id.ToString(),
                                     Text = d.nombre
                                 }).ToList();
            }

            //declaro la lista de dias para devilver a la vista
            List<SelectListItem> listaDias = new List<SelectListItem>();
            listaDias.Add(new SelectListItem() { Text = "Elija un dia..", Value = "0" });
            listaDias.Add(new SelectListItem() { Text = "Lunes", Value = "Monday" });
            listaDias.Add(new SelectListItem() { Text = "Martes", Value = "Tuesday" });
            listaDias.Add(new SelectListItem() { Text = "Miercoles", Value = "Wednesday" });
            listaDias.Add(new SelectListItem() { Text = "Jueves", Value = "Thursday" });
            listaDias.Add(new SelectListItem() { Text = "Viernes", Value = "Friday" });
            listaDias.Add(new SelectListItem() { Text = "Sabado", Value = "Saturday" });
            listaDias.Add(new SelectListItem() { Text = "Domingo", Value = "Sunday" });

            UnionModeloListayConsulta mymodelo = new UnionModeloListayConsulta();
            mymodelo.ListaLaboratorios = lst;
            mymodelo.ListaMaterias = listaMaterias;
            mymodelo.ListaDocentes = listaDocentes;
            mymodelo.ListaPeriodos = listaPeriodos;
            mymodelo.ListaDias = listaDias;

            mymodelo.Consulta = new ConsultaLaboratorioHorasViewModel();
            mymodelo.Registro = new InsertarRegistroVM();

            try
            {
                ViewBag.exito = TempData["exito"].ToString();
            }
            catch { }

            try
            {
                ViewBag.error = TempData["error"].ToString();
            }
            catch { }

            return View(mymodelo);
        }

        [HttpGet]
        public JsonResult getHorariosxLabyFecha(int idLab, DateTime fechaInicio, DateTime fechaFin)
        {
            List<RegistroVM> lst = new List<RegistroVM>();
            List<RegistroVM> ll = new List<RegistroVM>();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                List<registro> registros = db.registro.ToList();
                List<materia> materias = db.materia.ToList();
                List<docente> docentes = db.docente.ToList();

                ll = (from r in registros
                      join m in materias on r.idMateria equals m.id into table1
                      from m in table1.ToList()
                      join d in docentes on m.idDocente equals d.id into table2
                      from d in table2.ToList()
                      where r.fechaInicio >= fechaInicio && r.fechaFin <= fechaFin && r.idLaboratorio == idLab
                      select new RegistroVM
                      {
                          id = r.id,
                          fechaInicio = r.fechaInicio.ToString(),
                          fechaFin = r.fechaFin.ToString(),
                          tema = r.tema,
                          idLaboratorio = Convert.ToInt32(r.idLaboratorio),
                          idMateria = Convert.ToInt32(r.idMateria),
                          nombreMateria = m.nombre,
                          nombreDocente = d.nombre + " " + d.apellido
                      }).ToList();

            }
            return Json(ll, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult nuevoRegistro(UnionModeloListayConsulta modelo)
        {

            int cuantosErroresUbo = 0;

            if (modelo.Registro.Fecha == null && modelo.Registro.DiaSemana == "0")
            {
                TempData["error"] = "La fecha es incorrecta!";
                ViewBag.error = TempData["error"];

                return RedirectToAction("LaboratoriosGeneral");
            }

            try
            {

                if (modelo.Registro.DiaSemana == "0")
                {
                    DateTime fechaInicio = DateTime.ParseExact(modelo.Registro.Fecha + " " + modelo.Registro.horaInicio, "yyyy-MM-dd HH:mm", null);
                    DateTime fechaFin = DateTime.ParseExact(modelo.Registro.Fecha + " " + modelo.Registro.horaFin, "yyyy-MM-dd HH:mm", null);

                    if ((fechaFin <= fechaInicio))
                    {
                        TempData["error"] = "El horario ingresado es incorrecto!";
                        ViewBag.error = TempData["error"];

                        return RedirectToAction("LaboratoriosGeneral");
                    }

                    using (registroLabsEntities db = new registroLabsEntities())
                    {

                        bool hayError = hayErrorenHorario(fechaInicio, fechaFin, modelo);

                        //la lista tiene algo  ?
                        if (hayError)
                        {
                            TempData["error"] = "Ese horario ya esta en uso!";
                            ViewBag.error = TempData["error"];

                            return RedirectToAction("LaboratoriosGeneral");
                        }

                        var rTabla = new registro();
                        rTabla.fechaInicio = fechaInicio;
                        rTabla.fechaFin = fechaFin;
                        rTabla.tema = modelo.Registro.Tema;
                        rTabla.idLaboratorio = modelo.Registro.IdLaboratorio;
                        rTabla.observacion = modelo.Registro.Observacion;
                        rTabla.idMateria = modelo.Registro.IdMateria;


                        db.registro.Add(rTabla);
                        db.SaveChanges();

                        TempData["exito"] = "Registro guardado correctamente!";
                        ViewBag.exito = TempData["exito"];

                        return RedirectToAction("LaboratoriosGeneral");
                    }
                }
                else
                {

                    //primero obtengo el periodo
                    using (registroLabsEntities db = new registroLabsEntities())
                    {
                        PeriodoSimple periodo = new PeriodoSimple();

                        periodo = (from d in db.periodo
                                   where d.id == modelo.Registro.idPeriodo
                                   select new PeriodoSimple
                                   {
                                       Id = d.id,
                                       Nombre = d.nombre,
                                       FechaInicio = (DateTime)d.fechaInicio,
                                       FechaFin = (DateTime)d.fechaFin

                                   }).FirstOrDefault();
                        if (periodo == null)
                        {
                            TempData["error"] = "Error al encontrar el periodo indicado!";
                            ViewBag.error = TempData["error"];

                            return RedirectToAction("LaboratoriosGeneral");
                        }

                        //saco los lunes "o dia enviado" de entre esas dos fechas
                        List<string> listaFechas = GetDates(periodo.FechaInicio, periodo.FechaFin, modelo.Registro.DiaSemana);

                        //reviso que todos esten sin errores si hay no se inserta ese y continua
                        foreach (var item in listaFechas)
                        {
                            DateTime fechaInicio = DateTime.ParseExact(item + " " + modelo.Registro.horaInicio, "yyyy-MM-dd HH:mm", null);
                            DateTime fechaFin = DateTime.ParseExact(item + " " + modelo.Registro.horaFin, "yyyy-MM-dd HH:mm", null);

                            bool hayError = hayErrorenHorario(fechaInicio, fechaFin, modelo);

                            //la lista tiene algo  ?
                            if (hayError)
                            {
                                cuantosErroresUbo += 1;
                            }
                            else
                            {
                                //inserto el mismo registro con diferentes fechas y mismas horas
                                var rTabla = new registro();
                                rTabla.fechaInicio = fechaInicio;
                                rTabla.fechaFin = fechaFin;
                                rTabla.tema = modelo.Registro.Tema;
                                rTabla.idLaboratorio = modelo.Registro.IdLaboratorio;
                                //rTabla.observacion = modelo.Registro.Observacion;
                                rTabla.idMateria = modelo.Registro.IdMateria;


                                db.registro.Add(rTabla);
                                db.SaveChanges();
                            }
                        }

                        if (cuantosErroresUbo == 0)
                        {
                            TempData["exito"] = "Todos los registros programados se insertaron correctamente";
                            ViewBag.exito = TempData["exito"];
                            return RedirectToAction("LaboratoriosGeneral");
                        }
                        else if (cuantosErroresUbo == listaFechas.Count())
                        {
                            TempData["error"] = "El horario indicado se encuentra ocupado!";
                            ViewBag.error = TempData["error"];

                            return RedirectToAction("LaboratoriosGeneral");
                        }
                        else
                        {
                            TempData["error"] = "Algunos registros presentaron incovenientes de horario!";
                            ViewBag.error = TempData["error"];

                            return RedirectToAction("LaboratoriosGeneral");
                        }

                    }
                    
                }

            }
            catch (Exception ex)
            {
                //string r = "error";

                //return Json(ex.Message);
                //Redirect("Laboratorio/LaboratoriosGeneral/");
                throw new Exception(ex.Message);
            }
            //Redirect("Laboratorio/LaboratoriosGeneral/");
        }

        public ActionResult reporteAsistencias(int id)
        {
            //variables para el join de detalle registro


            //puedo hacer un join de todo esto pero me parece mas claro hacerlo asi 
            //modelo general a deviolver
            ModeloReporteAsistencias modeloReporte = new ModeloReporteAsistencias();
            //variable para guardar el regitro enviado
            RegistroSimpleVM registro = new RegistroSimpleVM();
            //variable para obtener la Materia
            MateriaVM materia = new MateriaVM();
            //variable para obtener el docente
            DocenteSimple docente = new DocenteSimple();
            //variable para el semestre
            SemestreSimple semestre = new SemestreSimple();
            //variable para la carrera
            CarreraSimple carrera = new CarreraSimple();
            //variable para el periodo
            PeriodoSimple periodo = new PeriodoSimple();
            //variable para obtener el laboratorio
            LaboratorioSimple laboratorio = new LaboratorioSimple();
            //variable para la lista de estudiantes
            List<DetalleRegistroEstudianteInfo> listaEstudiantes = new List<DetalleRegistroEstudianteInfo>();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                var registroTemporal = db.registro.Find(id);

                registro.Id = registroTemporal.id;
                registro.FechaInicio = (DateTime)registroTemporal.fechaInicio;
                registro.FechaFin = (DateTime)registroTemporal.fechaFin;
                registro.Tema = registroTemporal.tema;
                registro.IdLaboratorio = (int)registroTemporal.idLaboratorio;
                registro.Observacion = registroTemporal.observacion;
                registro.IdMateria = (int)registroTemporal.idMateria;

                materia = (from m in db.materia
                           where m.id == registro.IdMateria
                           select new MateriaVM
                           {
                               Id = m.id,
                               Nombre = m.nombre,
                               IdDocente = (int)m.idDocente,
                               IdSemestre = (int)m.idSemestre
                           }).FirstOrDefault();

                docente = (from d in db.docente
                           where d.id == materia.IdDocente
                           select new DocenteSimple
                           {
                               Id = d.id,
                               Nombre = d.nombre,
                               Apellido = d.apellido,
                               Cedula = d.cedula

                           }).FirstOrDefault();

                semestre = (from s in db.semestre
                            where s.id == materia.IdSemestre
                            select new SemestreSimple
                            {
                                Id = s.id,
                                Nombre = s.nombre,
                                IdCarrera = (int)s.idCarrera

                            }).FirstOrDefault();

                carrera = (from c in db.carrera
                           where c.id == semestre.IdCarrera
                           select new CarreraSimple
                           {
                               Id = c.id,
                               Nombre = c.nombre,
                               IdPerdiodo = (int)c.idPeriodo

                           }).FirstOrDefault();

                periodo = (from p in db.periodo
                           where p.id == carrera.IdPerdiodo
                           select new PeriodoSimple
                           {
                               Id = p.id,
                               Nombre = p.nombre,
                               FechaInicio = (DateTime)p.fechaInicio,
                               FechaFin = (DateTime)p.fechaFin

                           }).FirstOrDefault();

                laboratorio = (from l in db.laboratorio
                               where l.id == registro.IdLaboratorio
                               select new LaboratorioSimple
                               {
                                   Id = l.id,
                                   Nombre = l.nombre,
                                   Capacidad = (int)l.capacidad,
                                   Subred = (int)l.subred

                               }).FirstOrDefault();

                List<detalle_registro> detalleRegistros = db.detalle_registro.ToList();
                List<estudiante> estudiantes = db.estudiante.ToList();

                listaEstudiantes = (from dr in detalleRegistros
                                    join e in estudiantes on dr.idEstudiante equals e.id into table1
                                    from e in table1.ToList()
                                    where dr.idRegistro == registro.Id
                                    select new DetalleRegistroEstudianteInfo
                                    {
                                        NombreEstudiante = e.nombre,
                                        Observacion = dr.observacion
                                    }).ToList();



            };

            modeloReporte.Registro = registro;
            modeloReporte.Materia = materia;
            modeloReporte.Docente = docente;
            modeloReporte.Semestre = semestre;
            modeloReporte.Carrera = carrera;
            modeloReporte.Periodo = periodo;
            modeloReporte.Estudiantes = listaEstudiantes;
            modeloReporte.Laboratorio = laboratorio;


            return View(modeloReporte);
        }

        public ActionResult imprimirReporte(string cedula, int id)
        {

            return new ActionAsPdf("reporteAsistencias", new { id = id })
            {
                FileName = cedula + DateTime.Now.ToString("yyyyMMdd") + ".pdf",
                PageSize = Size.A4,
                CustomSwitches = "--print-media-type",
            };
        }


        //helper para controlar los horarios
        public bool hayErrorenHorario(DateTime fechaInicio, DateTime fechaFin, UnionModeloListayConsulta modelo)
        {
            using (registroLabsEntities db = new registroLabsEntities())
            {
                List<RegistroSimpleVM> existeRegistro = new List<RegistroSimpleVM>();

                existeRegistro = (from d in db.registro
                                  where (d.idLaboratorio == modelo.Registro.IdLaboratorio & fechaInicio == d.fechaInicio & fechaFin == d.fechaFin) ||
                                  (d.idLaboratorio == modelo.Registro.IdLaboratorio & fechaInicio > d.fechaInicio & fechaInicio < d.fechaFin) ||
                                  (d.idLaboratorio == modelo.Registro.IdLaboratorio & fechaFin > d.fechaInicio & fechaFin < d.fechaFin) ||
                                  (d.idLaboratorio == modelo.Registro.IdLaboratorio & fechaInicio == d.fechaInicio) ||
                                  (d.idLaboratorio == modelo.Registro.IdLaboratorio & fechaFin == d.fechaFin) ||
                                  (d.idLaboratorio == modelo.Registro.IdLaboratorio & d.fechaInicio > fechaInicio & d.fechaInicio < fechaFin) ||
                                  (d.idLaboratorio == modelo.Registro.IdLaboratorio & d.fechaFin > fechaInicio & d.fechaFin < fechaFin)
                                  select new RegistroSimpleVM
                                  {
                                      Id = d.id
                                  }).ToList();

                //la lista tiene algo  ?
                if (existeRegistro.Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //helper para obtener las fechas de un dia entre dos fechas
        static public List<string> GetDates(DateTime start_date, DateTime end_date, string dia)
        {
            List<string> days_list = new List<string>();
            for (DateTime date = start_date; date <= end_date; date = date.AddDays(1))
            {
                if (date.DayOfWeek.ToString() == dia)
                {
                    //Console.WriteLine(date.ToString("dd-MM-yyyy"));
                    days_list.Add(date.ToString("yyyy-MM-dd"));
                }

            }

            return days_list;
        }
    }
}
