using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] Character selectedCharacter;
    [SerializeField] Transform atkRef;
    [SerializeField] bool isBot;
    [SerializeField] List<Character> characterList;
    [SerializeField] UnityEvent onTakeDamage;
    [SerializeField] AudioClip damageClip;

    public Character SelectedCharacter { get => selectedCharacter; }
    public List<Character> CharacterList { get => characterList; }

    private void Start()
    {
        if (isBot)
        {
            foreach (var character in characterList)
            {
                character.Button.interactable = false;
            }
        }
    }
    public void Prepare()
    {
        selectedCharacter = null;
    }

    public void SelectCharacter(Character character)
    {
        selectedCharacter = character;
    }

    public void SetPlay(bool value)
    {
        if (isBot)
        {
            List<Character> lotteryList = new List<Character>();
            foreach (var character in characterList)
            {
                int ticket = Mathf.CeilToInt(((float)character.CurrentHP / (float)character.MaxHP) * 10);
                for (int i = 0; i < ticket; i++)
                {
                    lotteryList.Add(character);
                }
            }

            int index = Random.Range(0, lotteryList.Count);
            selectedCharacter = lotteryList[index];
        }
        else
        {
            foreach (var character in characterList)
            {
                character.Button.interactable = value;
            }
        }
    }

    public void Update()
    {
        if (direction == Vector3.zero)
            return;

        if (Vector3.Distance(selectedCharacter.transform.position, atkRef.position) > 0.1f)
        {
            selectedCharacter.transform.position += 10 * direction * Time.deltaTime;
        }
        else
        {
            direction = Vector3.zero;
            selectedCharacter.transform.position = atkRef.position;
        }
    }

    Vector3 direction = Vector3.zero;

    //fluent programing/syntax
    public void Attack()
    {
        selectedCharacter.transform.DOMove(atkRef.position, 0.5f);
        //.SetEase(Ease.InOutBounce);
    }

    public bool isAttacking()
    {
        if (selectedCharacter == null)
            return false;

        return DOTween.IsTweening(selectedCharacter.transform);
    }

    public void TakeDamage(int damageValue)
    {
        selectedCharacter.ChangeHP(-damageValue);
        var spriteRend = selectedCharacter.GetComponent<SpriteRenderer>();
        spriteRend.DOColor(Color.red, 0.1f).SetLoops(6, LoopType.Yoyo);
        onTakeDamage.Invoke();
    }

    public bool IsDamaging()
    {
        if (selectedCharacter == null)
            return false;

        var spriteRend = selectedCharacter.GetComponent<SpriteRenderer>();
        return DOTween.IsTweening(spriteRend);
    }

    public void Remove(Character character)
    {
        if (characterList.Contains(character) == false)
            return;

        if (selectedCharacter == character)
            selectedCharacter = null;

        character.Button.interactable = false;
        character.gameObject.SetActive(false);
        characterList.Remove(character);
    }

    public void Return()
    {
        selectedCharacter.transform.DOMove(selectedCharacter.InitialPosition, 0.5f);
    }

    public bool IsReturning()
    {
        if (selectedCharacter == null)
            return false;

        return DOTween.IsTweening(selectedCharacter.transform);
    }
}
