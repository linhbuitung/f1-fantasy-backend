﻿using F1Fantasy.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.StaticDataModule.Dtos
{
    public class ConstructorDto
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Nationality { get; set; }

        public string Code { get; set; }

        public string? ImgUrl { get; set; }

        public ConstructorDto(int id, string name, string nationality, string code, string? imgUrl)
        {
            Id = id;
            Name = name;
            Nationality = nationality;
            Code = code;
            ImgUrl = imgUrl;
        }

        public ConstructorDto(string name, string nationality, string code, string? imgUrl)
        {
            Id = null;
            Name = name;
            Nationality = nationality;
            Code = code;
            ImgUrl = imgUrl;
        }
    }
}