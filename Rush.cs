﻿using HREngine.API;
using HREngine.API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{
   public class Rushi : BotBase
   {
      protected override HRCard GetMinionByPriority(HRCard lastMinion)
      {
         HREntity result = null;
         if (HRPlayer.GetLocalPlayer().GetNumEnemyMinionsInPlay() <
            HRPlayer.GetLocalPlayer().GetNumFriendlyMinionsInPlay() ||
            HRPlayer.GetLocalPlayer().GetNumEnemyMinionsInPlay() < 4)
         {
            result = HRBattle.GetNextMinionByPriority(MinionPriority.Hero);
         }
         else
            result = HRBattle.GetNextMinionByPriority(MinionPriority.LowestHealth);

         if (result != null && (lastMinion == null || lastMinion != null && lastMinion.GetEntity().GetCardId() != result.GetCardId()))
            return result.GetCard();

         return null;
      }

      protected override int evaluatePlayfield(Playfield p)
      {
          int retval = 0;
          retval += p.owncards.Count * 1;

          retval += p.ownHeroHp + p.ownHeroDefence;
          retval += -(p.enemyHeroHp + p.enemyHeroDefence);

          retval += p.ownWeaponAttack;// +ownWeaponDurability;
          retval -= p.enemyWeaponDurability;

          retval += p.owncarddraw * 5;
          retval -= p.enemycarddraw * 5;

          retval += p.ownMaxMana;


          foreach (Action a in p.playactions)
          {
              if (!a.cardplay) continue;
              if (a.card.name == "hinrichten") retval -= 18; // a enemy minion make -10 for only being there, so + 10 for being eliminated 
              if (a.card.name == "flammenstoss" && a.numEnemysBeforePlayed <= 2) retval -= 20;
          }

          foreach (Minion m in p.ownMinions)
          {
              retval += m.Hp * 1;
              retval += m.Angr * 2;
              retval += m.card.rarity;
              if (m.windfury) retval += m.Angr;
              if (m.taunt) retval += 1;
          }

          foreach (Minion m in p.enemyMinions)
          {
              if (p.enemyMinions.Count >= 4)
              {
                  retval -= m.Hp;
                  retval -= m.Angr * 2;
                  if (m.windfury) retval -= m.Angr;
                  if (m.taunt) retval -= 5;
                  if (m.divineshild) retval -= 1;
                  if (m.frozen) retval += 1; // because its bad for enemy :D
                  if (m.poisonous) retval -= 4;
                  retval -= m.card.rarity;
              }


              
              if (m.name == "schlachtzugsleiter") retval -= 50;
              if (m.name == "grimmschuppenorakel") retval -= 50;
              if (m.name == "terrorwolfalpha") retval -= 20;
              if (m.name == "murlocanfuehrer") retval -= 50;
              if (m.name == "suedmeerkapitaen") retval -= 50;
              if (m.name == "championvonsturmwind") retval -= 100;
              if (m.name == "waldwolf") retval -= 50;
              if (m.name == "leokk") retval -= 50;
              if (m.name == "klerikerinvonnordhain") retval -= 50;
              if (m.name == "zauberlehrling") retval -= 30;
              if (m.name == "winzigebeschwoererin") retval -= 30;
              if (m.name == "beschwoerungsportal") retval -= 50;
              if (m.name == "aasfressendehyaene") retval -= 50;
              if (m.Angr >= 4) retval -= 50;
              if (m.Angr >= 7) retval -= 50;
          }

          retval -= p.enemySecretCount;
          retval -= p.lostDamage;//damage which was to high (like killing a 2/1 with an 3/3 -> => lostdamage =2
          retval -= p.lostWeaponDamage;
          if (p.ownMinions.Count == 0) retval -= 20;
          if (p.enemyMinions.Count >= 4) retval -= 200;
          if (p.enemyHeroHp <= 0) retval = 10000;
          if (p.enemyHeroHp >= 1 && p.ownHeroHp + p.ownHeroDefence - p.guessingHeroDamage <= 0) retval -= 1000;
          if (p.ownHeroHp <= 0) retval = -10000;

          p.value = retval;
          return retval;
      }
   }
}
