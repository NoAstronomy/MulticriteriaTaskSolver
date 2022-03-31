using AutoMapper;
using MulticriteriaTask.Solver.Db.Models;
using System;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskParameterHeader.ViewModels
{
    public class MulticriteriaTaskParameterHeaderBaseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UnitOfMeasurement { get; set; }
    }

    public class MulticriteriaTaskParameterHeaderBaseViewModelProfiler : Profile
    {
        public MulticriteriaTaskParameterHeaderBaseViewModelProfiler()
        {
            CreateMap<MulticriteriaTaskParameterHeaderModel, MulticriteriaTaskParameterHeaderBaseViewModel>();
        }
    }
}