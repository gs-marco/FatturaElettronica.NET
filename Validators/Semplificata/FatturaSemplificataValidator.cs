using System.Linq;
using FatturaElettronica.Semplificata;
using FluentValidation;

namespace FatturaElettronica.Validators.Semplificata
{
    public class FatturaSemplificataValidator : AbstractValidator<FatturaSemplificata>
    {
        public FatturaSemplificataValidator()
        {
            RuleFor(dt => dt.FatturaElettronicaHeader)
                .SetValidator(new FatturaElettronicaHeaderValidator());
            RuleForEach(dt => dt.FatturaElettronicaBody)
                .SetValidator(new FatturaElettronicaBodyValidator());
            RuleFor(x => x)
                .Must((fattura, _) => ImportoTotaleValidateAgainstError00460(fattura))
                .WithMessage("Importo totale superiore al limite previsto per le fatture semplificate ai sensi del DPR 633/72, art. 21bis e DM del 10 maggio 2019")
                .WithErrorCode("00460");
            RuleFor(x => x)
                .Must((fattura, _) => FatturaValidateAgainstError00471(fattura))
                .WithMessage(
                    "Il valore TD07 del tipo documento non ammette l’indicazione in fattura dello stesso soggetto sia come cedente che come cessionario")
                .WithErrorCode("00471");
        }

        private static bool FatturaValidateAgainstError00471(FatturaSemplificata fatturaSemplificata)
        {
            var cedente = fatturaSemplificata.FatturaElettronicaHeader.CedentePrestatore.IdFiscaleIVA.ToString();
            var cessionario =
                fatturaSemplificata.FatturaElettronicaHeader.CessionarioCommittente.IdentificativiFiscali.IdFiscaleIVA
                    .ToString();

            return cedente != cessionario || fatturaSemplificata.FatturaElettronicaBody.All(x =>
                x.DatiGenerali.DatiGeneraliDocumento.TipoDocumento != "TD07");
        }

        private static bool ImportoTotaleValidateAgainstError00460(FatturaSemplificata fatturaSemplificata)
        {
            var regimeFiscale = fatturaSemplificata.FatturaElettronicaHeader.CedentePrestatore.RegimeFiscale;
            if (regimeFiscale == "RF19" || regimeFiscale == "RF20")
                return true;

            return fatturaSemplificata.FatturaElettronicaBody.All(body =>
            {
                var importoTotale = body.DatiBeniServizi.Sum(x => x.Importo);

                if (importoTotale > 400)
                    return !body.DatiGenerali.DatiFatturaRettificata.IsEmpty();
                return true;
            });
        }
    }
}