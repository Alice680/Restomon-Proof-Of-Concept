using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuTeamBuilder : MenuSwapIcon
{
    [Serializable]
    private class BaseBuild
    {

    }

    [Serializable]
    private class EvolutionBuild
    {

    }

    [Serializable]
    private class CreatureBuild
    {
        public BaseBuild[] base_build;
        public EvolutionBuild[] evo_one_builds;
        public EvolutionBuild[] evo_two_builds;
        public EvolutionBuild[] evo_three_builds;
    }

    [SerializeField] private CreatureBuild[] creature_builds;
    [SerializeField] private Text[] text_boxes;

    public override void Activate()
    {
        base.Activate();

        Display();
    }

    public override void UpdateMenu(Direction dir)
    {

    }

    public void SetData(PermDataHolder data_holder)
    {

    }

    private void Display()
    {

    }
}