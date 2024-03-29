using FluentValidation;
using FluentValidation.Results;

namespace CursoMongo.Api.Domain.ValueObjects
{
    public class Avaliacao : AbstractValidator<Avaliacao>
    {
        public Avaliacao() {   }

        public Avaliacao(int estrelas, string comentario)
        {
            Estrelas = estrelas;
            Comentario = comentario;
        }

        public int Estrelas { get; private set; }
        public string Comentario { get; private set; }
        public ValidationResult ValidationResult { get; set; }

        public virtual bool Validar()
        {
            ValidarEstrelas();
            ValidarComentario();

            ValidationResult = Validate(this);

            return ValidationResult.IsValid;
        }

        private void ValidarComentario()
        {
            RuleFor(c => c.Comentario)
                .NotEmpty().WithMessage("Comentário não pode estar vazio.")
                .MaximumLength(100).WithMessage("Comentário opde ter no máximo 100 caracteres.");
        }

        private void ValidarEstrelas()
        {
            RuleFor(c => c.Estrelas)
                .GreaterThan(0).WithMessage("Número de estrelas deve ser maior que zero")
                .LessThanOrEqualTo(5).WithMessage("Número de estrelas deve ser menor ou igual a 5");
        }
    }
}