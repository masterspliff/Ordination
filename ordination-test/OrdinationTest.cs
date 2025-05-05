namespace ordination_test;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using shared.Model;
using System;
using System.Collections.Generic;

[TestClass]
public class OrdinationTest
{
    // Test data
    // Initialize with null! as they are guaranteed to be set in TestInitialize
    private Patient patient = null!;
    private Laegemiddel laegemiddel = null!;
    private DateTime startDato;
    private DateTime slutDato;

    [TestInitialize]
    public void SetupBeforeEachTest()
    {
        // Setup basic test data
        patient = new Patient("123456-7890", "Test Patient", 70.0);
        laegemiddel = new Laegemiddel("Acetylsalicylsyre", 0.1, 0.15, 0.16, "Styk");
        startDato = new DateTime(2023, 1, 10);
        slutDato = new DateTime(2023, 1, 20);
    }

    #region Ordination.antalDage Tests
    [TestMethod]
    public void AntalDage_StartOgSlutSammeDag_Returnerer1()
    {
        // Arrange
        var pn = new PN(new DateTime(2023, 1, 10), new DateTime(2023, 1, 10), 2, laegemiddel);
        
        // Act
        int antalDage = pn.antalDage();
        
        // Assert
        Assert.AreEqual(1, antalDage);
    }

    [TestMethod]
    public void AntalDage_10DagesMellemrum_Returnerer11()
    {
        // Arrange
        var pn = new PN(startDato, slutDato, 2, laegemiddel);
        
        // Act
        int antalDage = pn.antalDage();
        
        // Assert
        Assert.AreEqual(11, antalDage);
    }

    [TestMethod]
    public void AntalDage_StorPeriode_Returnerer61()
    {
        // Arrange - EK-3: Stor periode (≥ 30 dage)
        var storPeriodeStart = new DateTime(2023, 3, 1);
        var storPeriodeSlut = new DateTime(2023, 4, 30);
        var pn = new PN(storPeriodeStart, storPeriodeSlut, 2, laegemiddel);
        
        // Act
        int antalDage = pn.antalDage();
        
        // Assert
        Assert.AreEqual(61, antalDage);
    }
    #endregion

    #region DagligFast Tests
    [TestMethod]
    public void DagligFast_DoegnDosis_BeregnesKorrekt()
    {
        // Arrange
        double morgenAntal = 2;
        double middagAntal = 1;
        double aftenAntal = 0;
        double natAntal = 1;
        var dagligFast = new DagligFast(startDato, slutDato, laegemiddel, morgenAntal, middagAntal, aftenAntal, natAntal);
        
        // Act
        double doegnDosis = dagligFast.doegnDosis();
        
        // Assert
        Assert.AreEqual(morgenAntal + middagAntal + aftenAntal + natAntal, doegnDosis);
    }

    [TestMethod]
    public void DagligFast_SamletDosis_BeregnesKorrekt()
    {
        // Arrange
        double morgenAntal = 2;
        double middagAntal = 1;
        double aftenAntal = 0;
        double natAntal = 1;
        var dagligFast = new DagligFast(startDato, slutDato, laegemiddel, morgenAntal, middagAntal, aftenAntal, natAntal);
        double forventetDoegnDosis = morgenAntal + middagAntal + aftenAntal + natAntal;
        
        // Act
        double samletDosis = dagligFast.samletDosis();
        
        // Assert
        Assert.AreEqual(forventetDoegnDosis * 11, samletDosis); // 11 dage mellem 10/1 og 20/1 (inklusive)
    }

    [TestMethod]
    public void DagligFast_DoegnDosis_NulVedAlleDosisNul()
    {
        // Arrange - EK-4: DøgnDosis = 0
        double morgenAntal = 0;
        double middagAntal = 0;
        double aftenAntal = 0;
        double natAntal = 0;
        var dagligFast = new DagligFast(startDato, slutDato, laegemiddel, morgenAntal, middagAntal, aftenAntal, natAntal);
        
        // Act
        double doegnDosis = dagligFast.doegnDosis();
        
        // Assert
        Assert.AreEqual(0, doegnDosis);
    }

    [TestMethod]
    public void DagligFast_SamletDosis_NulVedAlleDosisNul()
    {
        // Arrange - EK-4: DøgnDosis = 0
        double morgenAntal = 0;
        double middagAntal = 0;
        double aftenAntal = 0;
        double natAntal = 0;
        var dagligFast = new DagligFast(startDato, slutDato, laegemiddel, morgenAntal, middagAntal, aftenAntal, natAntal);
        
        // Act
        double samletDosis = dagligFast.samletDosis();
        
        // Assert
        Assert.AreEqual(0, samletDosis);
    }
    #endregion

    #region DagligSkæv Tests
    [TestMethod]
    public void DagligSkaev_DoegnDosis_BeregnesKorrekt()
    {
        // Arrange
        var dagligSkaev = new DagligSkæv(startDato, slutDato, laegemiddel);
        dagligSkaev.opretDosis(new DateTime(1, 1, 1, 9, 0, 0), 2);
        dagligSkaev.opretDosis(new DateTime(1, 1, 1, 13, 0, 0), 1);
        dagligSkaev.opretDosis(new DateTime(1, 1, 1, 19, 0, 0), 3);
        
        // Act
        double doegnDosis = dagligSkaev.doegnDosis();
        
        // Assert
        Assert.AreEqual(6, doegnDosis); // 2 + 1 + 3 = 6
    }

    [TestMethod]
    public void DagligSkaev_SamletDosis_BeregnesKorrekt()
    {
        // Arrange
        var dagligSkaev = new DagligSkæv(startDato, slutDato, laegemiddel);
        dagligSkaev.opretDosis(new DateTime(1, 1, 1, 9, 0, 0), 2);
        dagligSkaev.opretDosis(new DateTime(1, 1, 1, 13, 0, 0), 1);
        dagligSkaev.opretDosis(new DateTime(1, 1, 1, 19, 0, 0), 3);
        
        // Act
        double samletDosis = dagligSkaev.samletDosis();
        
        // Assert
        Assert.AreEqual(66, samletDosis); // (2 + 1 + 3) * 11 dage = 6 * 11 = 66
    }
    #endregion

    #region PN Tests
    [TestMethod]
    public void PN_GivDosis_ReturnererTrue_NaarDatoIndenforPeriode()
    {
        // Arrange
        var pn = new PN(startDato, slutDato, 2, laegemiddel);
        var dato = new Dato() { dato = new DateTime(2023, 1, 15) };
        
        // Act
        bool result = pn.givDosis(dato);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(1, pn.getAntalGangeGivet());
    }

    [TestMethod]
    public void PN_DoegnDosis_FlydendeTal()
    {
        // Arrange - EK-5: DøgnDosis > 0, flydende tal
        var pn = new PN(startDato, slutDato, 1.5, laegemiddel);
        
        // Giv dosis 3 gange over 5 dage
        pn.givDosis(new Dato() { dato = new DateTime(2023, 1, 12) });
        pn.givDosis(new Dato() { dato = new DateTime(2023, 1, 14) });
        pn.givDosis(new Dato() { dato = new DateTime(2023, 1, 16) });
        
        // Act
        double doegnDosis = pn.doegnDosis();
        
        // Assert - 3 gange * 1.5 ml på 5 dage → døgnDosis = 0.9
        Assert.AreEqual(0.9, doegnDosis, 0.001);
    }

    [TestMethod]
    public void PN_GivDosis_ReturnererFalse_NaarDatoUdenforPeriode()
    {
        // Arrange
        var pn = new PN(startDato, slutDato, 2, laegemiddel);
        var dato = new Dato() { dato = new DateTime(2023, 1, 5) }; // Før startdato
        
        // Act
        bool result = pn.givDosis(dato);
        
        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(0, pn.getAntalGangeGivet());
    }

    [TestMethod]
    public void PN_DoegnDosis_BeregnesKorrekt_NaarGivetEnGang()
    {
        // Arrange
        var pn = new PN(startDato, slutDato, 2, laegemiddel);
        var dato = new Dato() { dato = new DateTime(2023, 1, 15) };
        pn.givDosis(dato);
        
        // Act
        double doegnDosis = pn.doegnDosis();
        
        // Assert
        Assert.AreEqual(2.0 / 1, doegnDosis); // 2 enheder / 1 dag = 2
    }

    [TestMethod]
    public void PN_DoegnDosis_BeregnesKorrekt_NaarGivetFlereDage()
    {
        // Arrange
        var pn = new PN(startDato, slutDato, 2, laegemiddel);
        pn.givDosis(new Dato() { dato = new DateTime(2023, 1, 12) });
        pn.givDosis(new Dato() { dato = new DateTime(2023, 1, 14) });
        pn.givDosis(new Dato() { dato = new DateTime(2023, 1, 16) });
        
        // Act
        double doegnDosis = pn.doegnDosis();
        
        // Assert
        // 3 gange * 2 enheder / 5 dage (12-16 inklusive) = 6 / 5 = 1.2
        Assert.AreEqual(1.2, doegnDosis, 0.001);
    }

    [TestMethod]
    public void PN_SamletDosis_BeregnesKorrekt()
    {
        // Arrange
        var pn = new PN(startDato, slutDato, 2, laegemiddel);
        pn.givDosis(new Dato() { dato = new DateTime(2023, 1, 12) });
        pn.givDosis(new Dato() { dato = new DateTime(2023, 1, 14) });
        pn.givDosis(new Dato() { dato = new DateTime(2023, 1, 16) });
        
        // Act
        double samletDosis = pn.samletDosis();
        
        // Assert
        Assert.AreEqual(6, samletDosis); // 3 gange * 2 enheder = 6
    }
    #endregion

    #region Ugyldig Input Tests
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Ordination_StartDatoEfterSlutDato_KasterException()
    {
        // Arrange - EK-6: startDato > slutDato
        var startEfterSlut = new DateTime(2023, 1, 20);
        var slutFoerStart = new DateTime(2023, 1, 10);
        
        // Act - Should throw exception
        var pn = new PN(startEfterSlut, slutFoerStart, 2, laegemiddel);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PN_NegativDosis_KasterException()
    {
        // Arrange & Act - EK-7: Dosis ≤ 0 (negativ/el. 0 i PN)
        var pn = new PN(startDato, slutDato, -2, laegemiddel);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Ordination_ManglendeReference_KasterException()
    {
        // Arrange & Act - EK-8: Manglende lægemiddel-reference
        // Suppress the warning for intentionally passing null to a non-nullable parameter for this test
        #pragma warning disable CS8625 
        var pn = new PN(startDato, slutDato, 2, null);
        #pragma warning restore CS8625
    }
    #endregion
}
