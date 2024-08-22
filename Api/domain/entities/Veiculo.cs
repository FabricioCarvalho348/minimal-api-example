﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_api.domain.entities;

public class Veiculo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } = default!;
    
    [Required]
    [StringLength(150)]
    public string Nome { get; set; } = default!;
    
    [Required]
    [StringLength(50)]
    public string Marca { get; set; } = default!;
    
    [Required]
    [StringLength(20)]
    public string Ano { get; set; } = default!;
}