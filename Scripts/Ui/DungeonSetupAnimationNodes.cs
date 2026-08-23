using System.Collections.Generic;
using Godot;

namespace NightFall.Scripts.Ui;

public sealed class DungeonSetupAnimationNodes(Node root)
{
    public Control MainLayout { get; } = root.GetNode<Control>(
        "../CenterContainer/MainLayout");

    public Control SetupPanel { get; } = root.GetNode<Control>(
        "../CenterContainer/MainLayout/SetupPanel");

    public Control PreviewPanel { get; } = root.GetNode<Control>(
        "../CenterContainer/MainLayout/PreviewPanel");

    public ColorRect TopLine { get; } = root.GetNode<ColorRect>(
        "../TopLine");

    public ColorRect SideRule { get; } = root.GetNode<ColorRect>(
        "../SideRule");

    public ColorRect HorizonGlow { get; } = root.GetNode<ColorRect>(
        "../HorizonGlow");

    public Label SetupEyebrow { get; } = root.GetNode<Label>(
        "../CenterContainer/MainLayout/SetupPanel/Content/Eyebrow");

    public Label SetupTitle { get; } = root.GetNode<Label>(
        "../CenterContainer/MainLayout/SetupPanel/Content/Title");

    public ColorRect TitleRule { get; } = root.GetNode<ColorRect>(
        "../CenterContainer/MainLayout/SetupPanel/Content/TitleRule");

    public Label PreviewEyebrow { get; } = root.GetNode<Label>(
        "../CenterContainer/MainLayout/PreviewPanel/Content/Eyebrow");

    public Label PreviewTitle { get; } = root.GetNode<Label>(
        "../CenterContainer/MainLayout/PreviewPanel/Content/Title");

    public ColorRect PreviewRule { get; } = root.GetNode<ColorRect>(
        "../CenterContainer/MainLayout/PreviewPanel/Content/Rule");

    public IReadOnlyList<Button> ModifierButtons { get; } =
    [
        root.GetNode<Button>(
            "../CenterContainer/MainLayout/SetupPanel/Content/ModifierPanel/ModifierMargin/ModifierList/BloodMoon"),

        root.GetNode<Button>(
            "../CenterContainer/MainLayout/SetupPanel/Content/ModifierPanel/ModifierMargin/ModifierList/GlassCannon"),

        root.GetNode<Button>(
            "../CenterContainer/MainLayout/SetupPanel/Content/ModifierPanel/ModifierMargin/ModifierList/HardNight"),

        root.GetNode<Button>(
            "../CenterContainer/MainLayout/SetupPanel/Content/ModifierPanel/ModifierMargin/ModifierList/Greed"),

        root.GetNode<Button>(
            "../CenterContainer/MainLayout/SetupPanel/Content/ModifierPanel/ModifierMargin/ModifierList/Fragile")
    ];

    public Button BackButton { get; } = root.GetNode<Button>(
        "../CenterContainer/MainLayout/SetupPanel/Content/Buttons/BackButton");

    public Button StartButton { get; } = root.GetNode<Button>(
        "../CenterContainer/MainLayout/SetupPanel/Content/Buttons/StartButton");

    public Label SkipLabel { get; } = root.GetNode<Label>("../SkipHint");
}