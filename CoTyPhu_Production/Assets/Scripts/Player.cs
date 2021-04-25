using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDiceListener
{
    [SerializeField] int _id;
    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }
    bool _isBroke;
    bool _notSubcribeDice = true;
    [SerializeField] bool minePlayer;
    [SerializeField] Button btnRoll;

    // Start is called before the first frame update
    void Start()
    {
        if (Dice.Ins() != null)
        {
            Dice.Ins().SubscribeDiceListener(this);
            _notSubcribeDice = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_notSubcribeDice)
        {
            Dice.Ins().SubscribeDiceListener(this);
            _notSubcribeDice = false;
        }
    }

    public void MoveTo(int plotID)
    {

    }

    public void StartPhase(int phaseID)
    {
        if(phaseID == 1 && minePlayer)
        {
            btnRoll.gameObject.SetActive(true);
        }
    }

    private void StartPhaseDice()
    {

    }

    public void EndPhase()
    {
        
    }

    public void PausePhase()
    {

    }

    public void Roll()
    {
        Dice.Ins().Roll(_id);

        if(TurnDirector.Ins.IsMyTurn(Id))
        {
            btnRoll.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// This function receive callback result from Dice when its finish rolling
    /// </summary>
    /// <param name="idPlayer"></param>
    /// <param name="result"></param>
    public void OnRoll(int idPlayer, List<int> result)
    {
        Debug.Log(result.ToArray());

        /// Do some fancy animation here

        // only the one who roll & that is control by me can announce end of phase
        if(idPlayer == Id && minePlayer)
        {
            Debug.Log("end of phase");
            TurnDirector.Ins.EndOfPhase();
        }
    }
}
