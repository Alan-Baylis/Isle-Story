﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonList : MonoBehaviour {
    public List<Person> people = new List<Person>();
    public List<PersonIndicator> buttons=new List<PersonIndicator>();
    Queue<PersonIndicator> buttonPool=new Queue<PersonIndicator>();
    public Transform contentList;
    public GameObject listPrefab;
    public Selector selector;
    public void ClearList() {
        for(int i=0; i < people.Count; i++) {
            RemoveButton(0);
        }
        people.Clear();
    }

    void RemoveButton(int index) {
        buttons[index].gameObject.SetActive(false);
        buttons[index].button.onClick.RemoveAllListeners();
        buttonPool.Enqueue(buttons[index]);
        buttons.RemoveAt(index);
    }

    void AddButton(int index) {
        PersonIndicator ind =
            buttonPool.Count > 0 ?
            buttonPool.Dequeue()
            : Instantiate(listPrefab, contentList).GetComponent<PersonIndicator>();
        buttons.Add(ind);
        ind.gameObject.SetActive(true);
        RectTransform tp = (RectTransform)buttons[index].gameObject.transform;
        Vector3 tV = tp.localPosition;
        tV.y = -30 * index + 75;
        tp.localPosition = tV;
        buttons[index].name.text = people[index].UIName;
        buttons[index].gender.text = people[index].gender ? "male" : "female";
        buttons[index].selector = selector;
        buttons[index].target = people[index];
        buttons[index].button.onClick.AddListener(buttons[index].SelectUnit);
    }
    public void AddPerson(Person person) {
        int t = people.Count;
        people.Add(person);
        AddButton(t);

    }
    public void RemovePerson(Person person) {
        int i = people.IndexOf(person);
        people.Remove(person);
        RemoveButton(i);
    }
    public void SetActive(bool val) {
        gameObject.SetActive(val);
        if (!val) ClearList();
    }
}
