﻿using MatriculasPrefeitura.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MatriculasPrefeitura.DAL
{
    public class CursoDAO
    {
        private static Context context = SingletonContext.GetInstance();

        public static List<Curso> RetornarCursos()
        {
            return context.Cursos.Include("Categoria").ToList();
        }

        public static bool CadastrarCurso(Curso curso)
        {
            if (BuscarCursoPorNome(curso) == null)
            {
                context.Cursos.Add(curso);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public static Curso BuscarCursoPorId(int id)
        {
            return context.Cursos.Find(id);
        }

        public static Curso BuscarCursoPorId(Curso curso)
        {
            return context.Cursos.FirstOrDefault(x => x.CursoId.Equals(curso.CursoId));
        }

        public static void RemoverCurso(int id)
        {
            context.Cursos.Remove(BuscarCursoPorId(id));
            context.SaveChanges();
        }

        public static bool AlterarCurso(Curso curso)
        {
            if (context.Cursos.Include("Professor").Include("Categoria").FirstOrDefault(x => x.CursoId == curso.CursoId && x.NomeCurso == curso.NomeCurso) == null)
            {
                var categoria = context.Categorias.First();
                var professor = context.Professores.First();
                curso.Categoria = categoria;
                curso.Professor = professor;
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public static Curso BuscarCursoPorNome(Curso curso)
        {
            return context.Cursos.Include("Categoria").FirstOrDefault(x => x.NomeCurso.Equals(curso.NomeCurso));
        }

        public static List<Curso> BuscarCursoPorCategoria(int? id)
        {
            return context.Cursos.Include("Categoria").Where(x => x.Categoria.CategoriaId == id).ToList();
        }

        public static List<Curso> ListarAlunos(int id)
        {
            return context.Cursos.Include("Categoria").Where(x => x.CursoId == id).ToList();
        }

        public static List<Matricula> ListarAlunoPorCurso(int id)
        {
            return context.Matriculas.Include("AlunoMatriculado").Where(x => x.CursoMatriculado.CursoId == id).ToList();
        }
    }
}