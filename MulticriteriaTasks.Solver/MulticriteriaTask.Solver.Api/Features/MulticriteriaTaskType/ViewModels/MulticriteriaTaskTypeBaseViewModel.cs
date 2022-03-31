using AutoMapper;
using MulticriteriaTask.Solver.Db.Models;
using System;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskType.ViewModels
{
    public class MulticriteriaTaskTypeBaseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class MulticriteriaTaskTypeBaseViewModelProfiler : Profile
    {
        public MulticriteriaTaskTypeBaseViewModelProfiler()
        {
            CreateMap<MulticriteriaTaskTypeModel, MulticriteriaTaskTypeBaseViewModel>();
        }
    }
}