using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class holds a list of monsters that are currently in the party
// currently set a max party size but maybe this should be somewhere else    jrv
public class Party
{
    public const int MAX_PARTY_MEMBERS = 4;
    List<Monster> partyMembers;

    public Party()
    {
        partyMembers = new List<Monster>();
    }

    public void addMember(Monster mon)
    {
        if(partyMembers.Count < MAX_PARTY_MEMBERS)
            partyMembers.Add(mon);
    }

    public void removeMember(Monster mon)
    {
        partyMembers.Remove(mon);
    }
}
