using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scatter.Api.Models;
using Scatter.Helpers;
using Scatter.World;
using UnityEngine;
using static Scatter.Helpers.PointerHelper;

public class HelpersTests
{
    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(6)]
    [TestCase(7)]
    [TestCase(8)]
    [TestCase(9)]
    [TestCase(10)]

    public void Generate_GenerateSpriteColliderWithLibraryHelper_CreatesCollider(int selected)
    {
        // Arrange
        var gameObject = new GameObject();
        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        var polygonCollider = gameObject.AddComponent<PolygonCollider2D>();

        Sprite[] textures = Resources.LoadAll<Sprite>("TestSprites");
        Sprite sprite = textures[selected];
        spriteRenderer.sprite = sprite;

        // Act
        LibraryHelper.GenerateSpriteCollider(gameObject);

        List<Vector2> spriteVertices = new List<Vector2>();
        spriteRenderer.sprite.GetPhysicsShape(0, spriteVertices);

        // Assert
        Assert.AreEqual(spriteVertices, polygonCollider.points);
    }

    [Test]
    [TestCase("Move")]
    [TestCase("Scale")]
    [TestCase("Rotate")]
    [TestCase("Erase")]
    [TestCase("Click")]
    [TestCase("Place")]
    public void Convert_ConvertStringToPointerMode_ReturnsPointerModeAsEnum(string mode)
    {
        // Arrange
        var rightMode = mode switch
        {
            "Move" => PointerMode.Move,
            "Scale" => PointerMode.Scale,
            "Rotate" => PointerMode.Rotate,
            "Erase" => PointerMode.Erase,
            "Click" => PointerMode.Click,
            "Place" => PointerMode.Place,
            _ => PointerMode.Move,
        };

        // Act
        var pointerMode = StringToPointerMode(mode);

        // Assert
        Assert.AreEqual(rightMode, pointerMode);
    }

    [Test]
    public void Convert_ConvertPlayerObjectToObject2D_ConvertsCorrectly()
    {
        // Arrange
        var environment2D = new Environment2D { id = "env1" };

        var playerObject = new GameObject().AddComponent<PlayerObject>();
        playerObject.gameObject.AddComponent<SpriteRenderer>();
        playerObject.ObjectId = "obj1";
        playerObject.PrefabId = "prefab1";
        playerObject.transform.position = new Vector3(1, 2, 0);
        playerObject.transform.localScale = new Vector3(3, 4, 1);
        playerObject.transform.rotation = Quaternion.Euler(0, 0, 45);
        playerObject.GetComponent<SpriteRenderer>().sortingOrder = 5;

        // Act  
        var object2D = ObjectHelper.ConvertPlayerObjectToObject2D(environment2D, playerObject);

        // Assert
        Assert.AreEqual(playerObject.ObjectId, object2D.id);
        Assert.AreEqual(environment2D.id, object2D.environmentId);
        Assert.AreEqual(playerObject.PrefabId, object2D.prefabId);
        Assert.AreEqual(playerObject.transform.position.x, object2D.positionX);
        Assert.AreEqual(playerObject.transform.position.y, object2D.positionY);
        Assert.AreEqual(playerObject.transform.localScale.x, object2D.scaleX);
        Assert.AreEqual(playerObject.transform.localScale.y, object2D.scaleY);
        Assert.AreEqual(playerObject.transform.rotation.eulerAngles.z, object2D.rotationZ);
        Assert.AreEqual(playerObject.GetComponent<SpriteRenderer>().sortingOrder, object2D.sortingLayer);
    }
}

