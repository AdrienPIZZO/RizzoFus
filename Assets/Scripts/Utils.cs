using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Utils
{
    public static int range(int x, int z, int x2, int z2){
        (int, int) vect = getVector(x, z, x2, z2);
        return Math.Abs(vect.Item1) + Math.Abs(vect.Item2);
    }
    public static (int, int) getVector(int x, int z, int x2, int z2){
        return (x2 - x, z2 - z);
    }

    public static List<(int, int)> getSquaresAtRange(int range, (int, int) position, Board board){
        List<(int, int)> result = new List<(int, int)>();
        if(range !=0){
            for(int i=0; i<range; i++){
                if(board.doesSquareExist(position.Item1 + i, position.Item2 + (range - i))) result.Add( (position.Item1 + i, position.Item2 + (range - i)) );
            }
            for(int i=0; i<range; i++){
                if(board.doesSquareExist(position.Item2 + (range - i), position.Item2 - i)) result.Add( (position.Item1 + (range - i), position.Item2 - i) );
            }
            for(int i=0; i<range; i++){
                if(board.doesSquareExist(position.Item1 - i, position.Item2 - (range - i))) result.Add( (position.Item1 - i, position.Item2 - (range - i)) );
            }
            for(int i=0; i<range; i++){
                if(board.doesSquareExist(position.Item1 - (range - i), position.Item2 + i)) result.Add( (position.Item1 - (range - i), position.Item2 + i) );
            }
        } else{
            result.Add(position);
        }
        return result;
    }
}

public class Fraction
{
    private int numerator;
    private int denominator;

    public Fraction(int numerator, int denominator)
    {
        if (denominator == 0)
        {
            throw new ArgumentException("Denominator cannot be zero.", nameof(denominator));
        }
        this.numerator = numerator;
        this.denominator = denominator;
    }

    //public static Fraction operator +(Fraction a) => a;
    public static Fraction operator -(Fraction a) => new Fraction(-a.numerator, a.denominator);

    public static Fraction operator +(Fraction a, Fraction b)
        => new Fraction(a.numerator * b.denominator + b.numerator * a.denominator, a.denominator * b.denominator);

    public static Fraction operator -(Fraction a, Fraction b)
        => a + (-b);
    public static Fraction operator +(int a, Fraction b)
        =>  new Fraction(a, 1) + b;

    public static Fraction operator -(int a, Fraction b)
        => new Fraction(a, 1) - b;

    public static Fraction operator *(Fraction a, Fraction b)
        => new Fraction(a.numerator * b.numerator, a.denominator * b.denominator);

    public static Fraction operator *(int a, Fraction b)
        => new Fraction(a * b.numerator, b.denominator);

    public static Fraction operator /(Fraction a, Fraction b)
    {
        if (b.numerator == 0)
        {
            throw new DivideByZeroException();
        }
        return new Fraction(a.numerator * b.denominator, a.denominator * b.numerator);
    }

    public double toDouble(){
        return (double) numerator / (double) denominator;
    }

    public override string ToString() => $"{numerator} / {denominator}";
}
