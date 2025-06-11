using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Sound;
using UnityEngine;

namespace Framework.Game.Defense
{
    public class CharacterChamy : Character
    {
        public enum UniqueValueType
        {
            MERGESUCCESS_RATE
        }
        
        public float mergeSuccessRate;

        public readonly string chamy_effet_name = "EfChamy_Fail";
        
        public override IEnumerator ActionSequence()
        {
            yield return null;
        }
        
        public override void SetCharacterInfo()
        {
            upgradeIndex = GameManager.Instance.characterSpawner.GetCharacterLevel(characterIndex);

            isAttackType = false;
            isThrow = characterData.isThrow;
            isSummoned = true;
            isNonFocus = true;

            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.missionManager.AddOrRemoveCharacter(this, true);
            }

            mergeSuccessRate = characterData.characterUniqueValue[(int)UniqueValueType.MERGESUCCESS_RATE] 
                               - starGradeIndex 
                               * (characterData.starFactor[0] 
                                  - characterData.classUpFactor[0] * characterData.characterClassLevel);
            
            bool isAppear = starGradeIndex == 0;

            AppearAction("Idle", isAppear);
        }



        public override void CharacterUpgradeSound(Character mergeCharacter)
        {
            if (mergeCharacter.characterIndex == this.characterIndex)
            {
                base.CharacterUpgradeSound(mergeCharacter);
            }
            else
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CHAMY_SUCCESS);
            }
        }
        
        public override void CharacterTypeCheckToMarge(Character character)
        {
            if (character.characterData.characterType != CharacterType.SUB &&
                character.starGradeIndex == this.starGradeIndex && starGradeIndex < maxStarGradeValue)
            {
                if (character.characterIndex == this.characterIndex)
                {
                    MergeCharacter(character);
                    transform.localPosition = character.pivotPosition;
            
                    isDrag = false;
                    if (characterInterface != null)
                    {
                        characterInterface.isDrag = false;
                    }
                }
                else
                {
                    float randomValue = Random.Range(0f, 100f);
                    bool isSuccess = randomValue <= mergeSuccessRate * 100f;
                    if (isSuccess)
                    {
                        MergeCharacter(character);
                    }
                    else
                    {
                        SoundManager.Instance.PlaySound(SoundKey.SF_CHAMY_FAIL);
                        ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(GetSkinParticleName(chamy_effet_name, skinID));
                        particle.PlayParticle(character.transform.localPosition, 0);
                        DestroyedCharacter();
                        character.DestroyedCharacter();
                    }
                }
            }
            else
            {
                transform.localPosition = pivotPosition;
            }
        }


        public override void SynthesisCharacter()
        {
            
        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            
        }
    }
}
