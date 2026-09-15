class A:
	const TARGET: = "wrong"

	class B:
		const TARGET: = "wrong"
		const WAITING: = "blackforest"

		class D extends C:
			pass

class C:
	const TARGET: = "right"

class E extends A.B.D:
	pass
