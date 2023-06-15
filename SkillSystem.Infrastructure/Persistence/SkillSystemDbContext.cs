using Microsoft.EntityFrameworkCore;
using SkillSystem.Core.Entities;

namespace SkillSystem.Infrastructure.Persistence;

public class SkillSystemDbContext : DbContext
{
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Duty> Duties { get; set; }
    public DbSet<Salary> Salaries { get; set;}
    public DbSet<GradeSkill> GradeSkills { get; set; }
    public DbSet<PositionGrade> PositionGrades { get; set; }
    public DbSet<PositionDuty> PositionDuties { get; set; }
    public DbSet<EmployeeSkill> EmployeeSkills { get; set; }
    public DbSet<EmployeeGrade> EmployeeGrades { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectRole> ProjectRoles { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<EmployeeInDepartment> EmployeesInDepartments { get; set; }
    public DbSet<SalaryRange> SalaryRanges { get; set; }
    public DbSet<SalaryTransaction> SalaryTransactions { get; set; }

    public SkillSystemDbContext()
    {
    }

    public SkillSystemDbContext(DbContextOptions<SkillSystemDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Skill>()
            .HasOne(skill => skill.Group)
            .WithMany(group => group.SubSkills)
            .HasForeignKey(skill => skill.GroupId);

        modelBuilder.Entity<Role>()
            .HasMany(role => role.Grades)
            .WithOne(grade => grade.Role)
            .HasForeignKey(grade => grade.RoleId);

        modelBuilder.Entity<Grade>(
            entity =>
            {
                entity
                    .HasMany(grade => grade.Skills)
                    .WithMany(skill => skill.Grades)
                    .UsingEntity<GradeSkill>(
                        joinEntity =>
                        {
                            joinEntity
                                .HasKey(gradeSkill => new { gradeSkill.GradeId, gradeSkill.SkillId });

                            joinEntity
                                .HasOne(gradeSkill => gradeSkill.Grade)
                                .WithMany(grade => grade.GradeSkills)
                                .HasForeignKey(gradeSkill => gradeSkill.GradeId);

                            joinEntity
                                .HasOne(gradeSkill => gradeSkill.Skill)
                                .WithMany(skill => skill.GradeSkills)
                                .HasForeignKey(gradeSkill => gradeSkill.SkillId)
                                .OnDelete(DeleteBehavior.Restrict);
                        });

                entity
                    .HasOne(grade => grade.PrevGrade)
                    .WithOne(prevGrade => prevGrade.NextGrade)
                    .HasForeignKey<Grade>(grade => grade.PrevGradeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

        modelBuilder.Entity<Position>()
            .HasMany(position => position.MinGrades)
            .WithMany(grade => grade.Positions)
            .UsingEntity<PositionGrade>(
                joinEntity =>
                {
                    joinEntity
                        .HasKey(positionGrade => new { positionGrade.PositionId, positionGrade.GradeId });

                    joinEntity
                        .HasOne(positionGrade => positionGrade.Position)
                        .WithMany(position => position.PositionGrades)
                        .HasForeignKey(positionGrade => positionGrade.PositionId);

                    joinEntity
                        .HasOne(positionGrade => positionGrade.Grade)
                        .WithMany(grade => grade.PositionGrades)
                        .HasForeignKey(positionGrade => positionGrade.GradeId)
                        .OnDelete(DeleteBehavior.Restrict);
                });

        modelBuilder.Entity<Position>()
            .HasMany(position => position.Duties)
            .WithMany(duty => duty.Positions)
            .UsingEntity<PositionDuty>(
                joinEntity =>
                {
                    joinEntity
                        .HasKey(positionDuty => new { positionDuty.PositionId, positionDuty.DutyId });

                    joinEntity
                        .HasOne(positionDuty => positionDuty.Position)
                        .WithMany(position => position.PositionDuties)
                        .HasForeignKey(positionDuty => positionDuty.PositionId);

                    joinEntity
                        .HasOne(positionDuty => positionDuty.Duty)
                        .WithMany(duty => duty.PositionDuties)
                        .HasForeignKey(positionDuty => positionDuty.DutyId)
                        .OnDelete(DeleteBehavior.Restrict);
                });

        modelBuilder.Entity<EmployeeSkill>()
            .HasKey(skill => new { skill.EmployeeId, skill.SkillId });

        modelBuilder.Entity<EmployeeSkill>()
            .HasOne(skill => skill.Employee)
            .WithMany()
            .HasForeignKey(skill => skill.EmployeeId);

        modelBuilder.Entity<EmployeeSkill>()
            .HasOne(skill => skill.Skill)
            .WithMany()
            .HasForeignKey(skill => skill.SkillId);

        modelBuilder.Entity<EmployeeGrade>()
            .HasKey(grade => new { grade.EmployeeId, grade.GradeId });

        modelBuilder.Entity<EmployeeGrade>()
            .HasOne(employeeGrade => employeeGrade.Employee)
            .WithMany()
            .HasForeignKey(employeeGrade => employeeGrade.EmployeeId);

        modelBuilder.Entity<EmployeeGrade>()
            .HasOne(employeeGrade => employeeGrade.Grade)
            .WithMany()
            .HasForeignKey(employeeGrade => employeeGrade.GradeId);

        modelBuilder.Entity<Employee>()
            .HasOne(employee => employee.Manager)
            .WithMany()
            .HasForeignKey(employee => employee.ManagerId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<Project>()
            .HasMany(project => project.Roles)
            .WithOne(role => role.Project)
            .HasForeignKey(role => role.ProjectId);

        modelBuilder.Entity<ProjectRole>()
            .HasOne(projectRole => projectRole.Role)
            .WithMany()
            .HasForeignKey(projectRole => projectRole.RoleId);

        modelBuilder.Entity<ProjectRole>()
            .HasOne(projectRole => projectRole.Employee)
            .WithMany()
            .HasForeignKey(projectRole => projectRole.EmployeeId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<Department>()
            .HasIndex(department => department.Name)
            .IsUnique();

        modelBuilder.Entity<Department>()
            .HasOne(department => department.HeadEmployee)
            .WithOne()
            .HasForeignKey<Department>(department => department.HeadEmployeeId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<Department>()
            .HasMany(department => department.Employees)
            .WithMany(employee => employee.Departments)
            .UsingEntity<EmployeeInDepartment>()
            .HasKey(employeeInDepartment => new { employeeInDepartment.DepartmentId, employeeInDepartment.EmployeeId });
    }
}
