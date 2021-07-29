using Registrov2._0.Models;
using Registrov2._0.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace Registrov2._0.Controllers
{
    public class RegistroController : Controller
    {
        // GET: Registro
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult eliminarRegistro(int idRegistro)
        {
            JsonRespuesta respuesta = new JsonRespuesta();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                try
                {
                    registro registroEliminar = db.registro.Find(idRegistro);
                    db.registro.Remove(registroEliminar);
                    db.SaveChanges();

                    respuesta.Status = "success";
                    respuesta.Mensaje = "Registro eliminado correctamente!";

                    return Json(respuesta, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    respuesta.Status = "error";
                    respuesta.Mensaje = "El registro no se elimino porque ya existen asistencias!";

                    return Json(respuesta, JsonRequestBehavior.AllowGet);
                }

            }
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult insertarAsistenciaenRegistro(string cedula, int idLaboratorio)
        {
            if ( cedula == "" || cedula == null  )
            {
                JsonRespuesta rst = new JsonRespuesta();
                rst.Status = "error";
                rst.Mensaje = "Porfavor ingrese su cedula!";

                return Json(rst, JsonRequestBehavior.AllowGet);

            }


            DateTime today = DateTime.Now;

            //variable para guardar el registro en caso de que exista
            RegistroSimpleVM registro = new RegistroSimpleVM();
            //variable para almacenar el estudiante en caso de que sea estudiantre
            EstudianteSimple estudiante = new EstudianteSimple();
            //variable para lamacenar el docente en caso de que sea docente
            DocenteSimple docente = new DocenteSimple();
            //variable para verificar si el estudiante toma esa materia
            DetalleMateriaSimple detalleMateria = new DetalleMateriaSimple();
            //variable para verificar que el docente imparta esa materia
            MateriaVM materia = new MateriaVM();
            
            //variable para devolver el objeto despues de haber pasado todos los filtros
            ModeloVerificacionAsistencia respuesta = new ModeloVerificacionAsistencia();

            string indicadorEstudianteDocente = "";

            using ( registroLabsEntities db = new registroLabsEntities() )
            {
                registro = (from d in db.registro
                            where today >= d.fechaInicio && today <= d.fechaFin && d.idLaboratorio == idLaboratorio
                            select new RegistroSimpleVM
                            {
                                Id = d.id,
                                FechaInicio = (DateTime)d.fechaInicio,
                                FechaFin = (DateTime)d.fechaFin,
                                Tema = d.tema,
                                IdLaboratorio =(int)(d.idLaboratorio),
                                Observacion = d.observacion,
                                IdMateria = (int)(d.idMateria)

                            }).FirstOrDefault();

                if (registro == null)
                {
                    JsonRespuesta rst = new JsonRespuesta();
                    rst.Status = "error";
                    rst.Mensaje = "No se encontro una clase agendada!";

                    return Json(rst, JsonRequestBehavior.AllowGet);
                }

                //verifico si la cedula pertenece a un estudiante o a un docente
                estudiante = (from e in db.estudiante
                              where e.cedula == cedula
                              select new EstudianteSimple
                              {
                                  Id = e.id,
                                  Cedula = e.cedula,
                                  Nombre = e.nombre

                              }).FirstOrDefault();

                if (estudiante == null)
                {

                    docente = (from d in db.docente
                               where d.cedula == cedula
                               select new DocenteSimple
                               {
                                   Id = d.id,
                                   Cedula = d.cedula.ToString(),
                                   Nombre = d.nombre,
                                   Apellido = d.apellido

                               }).FirstOrDefault();

                    if (docente == null)
                    {
                        JsonRespuesta rst = new JsonRespuesta();
                        rst.Status = "error";
                        rst.Mensaje = "No se encontro el usuario indicado!";

                        return Json(rst, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        indicadorEstudianteDocente = "docente";
                    }
                }
                else
                {
                    indicadorEstudianteDocente = "estudiante";
                }

                //despues de definir si es estudiante o docente la cedula ingresada valido
                //si ese estudiante toma esa materia
                //o si ese dicente da esa materia

                if (indicadorEstudianteDocente == "estudiante")
                {
                    //registro para el estudiante luego de la verificacion que toma esta materia
                    detalleMateria = (from dm in db.detalle_materia
                                      where dm.idEstudiante == estudiante.Id && dm.idMateria == registro.IdMateria
                                      select new DetalleMateriaSimple
                                      {
                                          Id = dm.id,
                                          IdMateria = (int)dm.idMateria,
                                          idEstudiante = (int) dm.idEstudiante

                                      }).FirstOrDefault();

                    if (detalleMateria == null)
                    {
                        //mando mensaje de error
                        JsonRespuesta rst = new JsonRespuesta();
                        rst.Status = "error";
                        rst.Mensaje = "<b>" + estudiante.Nombre+ ",</b></br> usted no toma la materia agendada!";

                        return Json(rst, JsonRequestBehavior.AllowGet);
                    }

                    //variable para verificar si no se ha registrado antes
                    DetalleRegistroSimple detalleRegistro = new DetalleRegistroSimple();
                    //debe hber un control de que si ya se registro
                    detalleRegistro = (from dr in db.detalle_registro
                                       where dr.idRegistro == registro.Id && dr.idEstudiante == estudiante.Id
                                       select new DetalleRegistroSimple {
                                           Id = dr.id,
                                           IdRegistro = (int)dr.idRegistro,
                                           IdEstudianteDocente = (int)dr.idEstudiante,
                                           Observacion = dr.observacion
                                       }).FirstOrDefault();

                    if (detalleRegistro != null)
                    {
                        //mando mensaje de error
                        JsonRespuesta rst = new JsonRespuesta();
                        rst.Status = "error";
                        rst.Mensaje = "<b>" + estudiante.Nombre + ",</b> </br> su registro ya fue realizado!";

                        return Json(rst, JsonRequestBehavior.AllowGet);
                    }


                } else if (indicadorEstudianteDocente == "docente")
                {
                    materia = (from m in db.materia
                               where m.id == registro.IdMateria && m.idDocente == docente.Id
                               select new MateriaVM
                               {
                                   Id = m.id,
                                   Nombre = m.nombre,
                                   IdDocente = (int)m.idDocente,
                                   IdSemestre = (int)m.idSemestre

                               }).FirstOrDefault();

                    if (materia == null)
                    {
                        //mando mensaje de error
                        JsonRespuesta rst = new JsonRespuesta();
                        rst.Status = "error";
                        rst.Mensaje = "<b>" + docente.Nombre +" "+ docente.Apellido  + ",</b> </br>usted no imparte la materia agendada!";

                        return Json(rst, JsonRequestBehavior.AllowGet);
                    }

                    //variable para verificar si no se ha registrado antes
                    DetalleRegistroSimple detalleRegistro = new DetalleRegistroSimple();
                    //debe hber un control de que si ya se registro
                    detalleRegistro = (from dr in db.detalle_registro
                                       where dr.idRegistro == registro.Id && dr.idDocente == docente.Id
                                       select new DetalleRegistroSimple
                                       {
                                           Id = dr.id,
                                           IdRegistro = (int)dr.idRegistro,
                                           IdEstudianteDocente = (int)dr.idDocente,
                                           Observacion = dr.observacion
                                       }).FirstOrDefault();

                    if (detalleRegistro != null)
                    {
                        //mando mensaje de error
                        JsonRespuesta rst = new JsonRespuesta();
                        rst.Status = "error";
                        rst.Mensaje = "<b>" + docente.Nombre + " " + docente.Apellido + ",</b> </br> su registro ya fue realizado!";

                        return Json(rst, JsonRequestBehavior.AllowGet);
                    }

                }


                List<materia> materias = db.materia.ToList();
                List<semestre> semestres = db.semestre.ToList();
                List<registro> registros = db.registro.ToList();
                //List<docente> docentes = db.docente.ToList();

                respuesta = (from r in registros
                             join m in materias on r.idMateria equals m.id into table1
                             from m in table1.ToList()
                             join s in semestres on m.idSemestre equals s.id into table2
                             from s in table2.ToList()
                             where r.id == registro.Id
                             select new ModeloVerificacionAsistencia
                             {
                                 IdMateria = m.id,
                                 NombreMateria = m.nombre,
                                 NombreSemestre = s.nombre,
                                 IdRegistro = r.id,
                                 TemaRegistro = r.tema

                             }).FirstOrDefault();


            }

            if (indicadorEstudianteDocente == "estudiante")
            {
                respuesta.TipoUsuario = "estudiante";
                respuesta.IdUsuario = estudiante.Id;
                respuesta.NombreUsuario = estudiante.Nombre;

            }
            else
            {
                respuesta.TipoUsuario = "docente";
                respuesta.IdUsuario = docente.Id;
                respuesta.NombreUsuario = docente.Nombre + " " +docente.Apellido;
            }


            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }
        //comprueba que no tenga errores sino inserta con el metodo de abajo

        [HttpGet]
        [AllowAnonymous]
        public JsonResult insertarRegistro( int idRegistro, string observacion, string tipoUsuario, int idUsuario, string tema )
        {
            //obtengo el nombre de la compu
            string host = Dns.GetHostName();
            //variable para enviar la respuesta
            JsonRespuesta rst = new JsonRespuesta();

            if (observacion == "")
            {
                observacion = host + " sn";
            }
            else
            {
                observacion = host + " " + observacion;
            }

            using ( registroLabsEntities db = new registroLabsEntities() )
            {
                var nuevaAsistencia = new detalle_registro();
                nuevaAsistencia.idRegistro = idRegistro;
                nuevaAsistencia.observacion = observacion;

                if (tipoUsuario == "estudiante")
                {
                    nuevaAsistencia.idEstudiante = idUsuario;
                }
                else
                {
                    nuevaAsistencia.idDocente = idUsuario;

                    //actualizar el tema
                    var actualizarRegistro = db.registro.Find(idRegistro);
                    actualizarRegistro.tema = tema;

                    db.Entry(actualizarRegistro).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    
                }

                try
                {
                    db.detalle_registro.Add(nuevaAsistencia);
                    db.SaveChanges();

                    rst.Status = "success";
                    rst.Mensaje = "Su asistencia fue registrada!";

                    return Json(rst, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    rst.Status = "error";
                    rst.Mensaje = "Algo salio mal intente mas tarde!";

                    return Json(rst, JsonRequestBehavior.AllowGet);
                }
                
            }
        }
    }
}
