using System.Collections;

public static class DoubleExtensions {
	public static double Lerp(double a, double b, double t) {
		return (1 - t) * a + t * b;
	}
}