/*******
Start these types are used when we want to take our courses and group them by areaOfInstruction with getPackageDetails
*******/
interface CourseAreaDetail {
  courseNumber: string;
  courseTitle: string;
  hours: string;
}

// The Map structure: Key is the ID, Value is the list of course details
type AreaOfInstructionWithCourseHoursMap = Map<string, CourseAreaDetail[]>;

/*******
End these types are used when we want to take our courses and group them by areaOfInstruction with getPackageDetails
*******/
