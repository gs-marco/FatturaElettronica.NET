using System;
using System.Collections.Generic;
using System.Linq;
using FatturaElettronica.Semplificata.FatturaElettronicaBody;
using FatturaElettronica.Semplificata.FatturaElettronicaBody.DatiBeniServizi;
using FluentValidation;

namespace FatturaElettronica.Validators.Semplificata
{
    public class FatturaElettronicaBodyValidator : AbstractValidator<FatturaElettronicaBody>
    {
        private static DateTime _20210101 = new DateTime(2021, 1, 1);

        public FatturaElettronicaBodyValidator()
        {
            RuleFor(x => x.DatiGenerali)
                .SetValidator(new DatiGeneraliValidator());
            RuleForEach(x => x.DatiBeniServizi)
                .SetValidator(new DatiBeniServiziValidator());
            RuleFor(x => x.DatiBeniServizi)
                .NotEmpty().WithMessage("DatiBeniServizi è obbligatorio");

            RuleForEach(x => x.Allegati)
                .SetValidator(new AllegatiValidator());

            RuleFor(x => x.DatiBeniServizi)
                .Must((body, _) => NaturaSemplificataAgainstError00445(body))
                .When(x => x.DatiGenerali.DatiGeneraliDocumento.Data.CompareTo(_20210101) >= 0)
                .WithMessage("A partire dal 01/01/2021 non è più ammesso il valore generico N2 o N3 come codice natura dell’operazione")
                .WithErrorCode("00445");
        }

        private static bool NaturaSemplificataAgainstError00445(FatturaElettronicaBody body)
        {
            var codiciNatura = new HashSet<string>() { "N2", "N3" };

            // NaturaLinea
            if (body.DatiBeniServizi
                .Where(l => !string.IsNullOrEmpty(l.Natura))
                .Any(l => codiciNatura.Contains(l.Natura)))
            {
                return false;
            }

            return true;
        }
    }
}