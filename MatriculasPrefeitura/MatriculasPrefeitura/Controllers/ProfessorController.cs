﻿using MatriculasPrefeitura.Models;
using MatriculasPrefeitura.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace MatriculasPrefeitura.Controllers
{
    public class ProfessorController : Controller
    {
        // GET: Professor
        public ActionResult Index()
        {
            return View(ProfessorDAO.RetornarProfessores());
        }

        public ActionResult CadastrarProfessor()
        {
            if (TempData["Mensagem"] != null)
            {
                ModelState.AddModelError("", TempData["Mensagem"].ToString());
            }
            return View((Professor)TempData["Professor"]);
        }


        [HttpPost]
        public ActionResult CadastrarProfessor(Professor professor, HttpPostedFileBase fupImagem)
        {
            if (ModelState.IsValid)
            {
                if (fupImagem != null)
                {
                    string nomeImagem = Path.GetFileName(fupImagem.FileName);
                    string caminho = Path.Combine(Server.MapPath("~/Images/"), nomeImagem);
                    fupImagem.SaveAs(caminho);
                    professor.FotoProfessor = nomeImagem;
                }
                else
                {
                    professor.FotoProfessor = "semImagem.jpg"; // ENCONTRAR FOTO SEM NADA PRA COLOCAR AQUI
                }

                if (ProfessorDAO.CadastrarProfessor(professor) == true)
                {
                    ModelState.AddModelError("", "Professor adicionado com sucesso!");
                    return View(professor);
                }
                else
                {
                    ModelState.AddModelError("", "Não é possível alterar um professor com o mesmo CPF!");
                    return View(professor);
                }

            }
            else
            {
                return View(professor);
            }

        }

        public ActionResult EditarProfessor(int id)
        {
            return View(ProfessorDAO.BuscarProfessorPorId(id));
        }

        [HttpPost]
        public ActionResult EditarProfessor(Professor professorAlterado)
        {
            Professor professorOriginal = ProfessorDAO.BuscarProfessorPorId(professorAlterado.NumProfessor);

            professorOriginal.NomeProfessor = professorAlterado.NomeProfessor;
            professorOriginal.CPFProfessor = professorAlterado.CPFProfessor;
            professorOriginal.FormacaoProfessor = professorAlterado.FormacaoProfessor;
            professorOriginal.FotoProfessor = professorAlterado.FotoProfessor;
            professorOriginal.Logradouro = professorAlterado.Logradouro;
            professorOriginal.CEP = professorAlterado.CEP;
            professorOriginal.Numero = professorAlterado.Numero;
            professorOriginal.Bairro = professorAlterado.Bairro;
            professorOriginal.Localidade = professorAlterado.Localidade;
            professorOriginal.UF = professorAlterado.UF;


            if (ModelState.IsValid)
            {
                if (ProfessorDAO.AlterarProfessor(professorOriginal))
                {
                    return RedirectToAction("Index", "Professor");
                }
                else
                {
                    ModelState.AddModelError("", "Não é possível alterar um professor com o mesmo nome!");
                    return View(professorOriginal);
                }
            }
            else
            {
                return View(professorOriginal);
            }
        }


        public ActionResult ExcluirProfessor(int id)
        {
            ProfessorDAO.ExcluirProfessor(id);
            return RedirectToAction("Index", "Professor");
        }

        [HttpPost]
        public ActionResult PesquisarCEP(Professor professor)
        {
            try
            {
                string url = "https://viacep.com.br/ws/" + professor.CEP + "/json/";

                WebClient client = new WebClient();
                string json = client.DownloadString(url);
                // Converter string pra UTF-8
                byte[] bytes = Encoding.Default.GetBytes(json);
                json = Encoding.UTF8.GetString(bytes);
                // Converter json para objeto
                professor = JsonConvert.DeserializeObject<Professor>(json);

                // Passar informação para qualquer action do controller
                TempData["Professor"] = professor;
            }
            catch (Exception)
            {
                TempData["Mensagem"] = "CEP Inválido!";
            }

            return RedirectToAction("CadastrarProfessor", "Professor");
        }
    }
}