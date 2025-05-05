namespace shared.Model;

public class PN : Ordination {
	public double antalEnheder { get; set; }
    public List<Dato> dates { get; set; } = new List<Dato>();

    public PN (DateTime startDen, DateTime slutDen, double antalEnheder, Laegemiddel laegemiddel) : base(laegemiddel, startDen, slutDen) {
        if (antalEnheder <= 0)
        {
            throw new ArgumentException("Antal enheder skal være større end 0");
        }
        
		this.antalEnheder = antalEnheder;
	}

    public PN() : base(null!, new DateTime(), new DateTime()) {
    }

    /// <summary>
    /// Registrerer at der er givet en dosis på dagen givesDen
    /// Returnerer true hvis givesDen er inden for ordinationens gyldighedsperiode og datoen huskes
    /// Returner false ellers og datoen givesDen ignoreres
    /// </summary>
    public bool givDosis(Dato givesDen) {
        if (givesDen.dato >= startDen && givesDen.dato <= slutDen) {
            dates.Add(givesDen);
            return true;
        }
        return false;
    }

    public override double doegnDosis() {
        if (dates.Count == 0) {
            return 0;
        }
        
        DateTime firstDate = dates.Min(d => d.dato);
        DateTime lastDate = dates.Max(d => d.dato);
        
        // If only one date, the period is 1 day
        int daysBetween = (int)(lastDate - firstDate).TotalDays + 1;
        
        return (dates.Count * antalEnheder) / daysBetween;
    }


    public override double samletDosis() {
        return dates.Count() * antalEnheder;
    }

    public int getAntalGangeGivet() {
        return dates.Count();
    }

	public override String getType() {
		return "PN";
	}
}
