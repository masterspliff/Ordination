namespace shared.Model;

public class DagligSkæv : Ordination {
    public List<Dosis> doser { get; set; } = new List<Dosis>();

    public DagligSkæv(DateTime startDen, DateTime slutDen, Laegemiddel laegemiddel) : base(laegemiddel, startDen, slutDen) {
	}

    public DagligSkæv(DateTime startDen, DateTime slutDen, Laegemiddel laegemiddel, Dosis[] doser) : base(laegemiddel, startDen, slutDen) {
        
        if (doser == null || doser.Length == 0)
        {
             throw new ArgumentException("Doseringslisten må ikke være tom.");
        }
        if (doser.Any(d => d.antal < 0))
        {
            throw new ArgumentException("Dosisantal må ikke være negativt.");
        }
        this.doser = doser.ToList();
    }

    public DagligSkæv() : base() {
         // Parameterless constructor for Entity Framework Core
    }

    public void opretDosis(DateTime tid, double antal) {
        if (antal < 0)
        {
            throw new ArgumentException("Dosisantal må ikke være negativt.");
        }
        doser.Add(new Dosis(tid, antal));
    }

	public override double samletDosis() {
		return base.antalDage() * doegnDosis();
	}

	public override double doegnDosis() {
		return doser.Sum(d => d.antal);
	}

	public override String getType() {
		return "DagligSkæv";
	}
}
