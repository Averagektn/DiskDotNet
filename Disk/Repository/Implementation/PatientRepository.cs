using Disk.Db.Context;
using Disk.Entities;
using Disk.Extensions;
using Disk.Properties.Langs.RepositoryException;
using Disk.Repository.Common.Implementation;
using Disk.Repository.Exceptions;
using Disk.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Disk.Repository.Implementation;

public class PatientRepository(DiskContext diskContext) : CrudRepository<Patient>(diskContext), IPatientRepository
{
    public new void Add(Patient entity)
    {
        var patientExists = Table
            .Any(p =>
                p.Name == entity.Name &&
                p.Surname == entity.Surname &&
                p.Patronymic == entity.Patronymic &&
                p.DateOfBirth == entity.DateOfBirth);

        if (patientExists)
        {
            throw new DuplicateEntityException("Patient duplication", RepositoryExceptionLocalization.ItemDuplication);
        }

        base.Add(entity);
    }

    public new async Task AddAsync(Patient entity)
    {
        var patientExists = await Table
            .AnyAsync(
                p =>
                p.Name == entity.Name &&
                p.Surname == entity.Surname &&
                p.Patronymic == entity.Patronymic &&
                p.DateOfBirth == entity.DateOfBirth);

        if (patientExists)
        {
            throw new DuplicateEntityException("Patient duplication", RepositoryExceptionLocalization.ItemDuplication);
        }

        await base.AddAsync(entity);
    }

    public ICollection<Patient> GetPatientsByFullname(string name, string surname, string patronymic)
    {
        name = name.CapitalizeFirstLetter();
        surname = surname.CapitalizeFirstLetter();
        patronymic = patronymic.CapitalizeFirstLetter();

        return [.. Table
            .Where(
                p =>
                p.Name.Contains(name) &&
                p.Surname.Contains(surname) &&
                (p.Patronymic == null || p.Patronymic.Contains(patronymic))
            )];
    }

    public long GetPatientsCount()
    {
        return Table.Count();
    }

    public ICollection<Patient> GetPatientsPage(int pageNum, int patientsPerPage)
    {
        return [.. Table
            .OrderBy(p => p.Surname)
            .Skip(pageNum * patientsPerPage)
            .Take(patientsPerPage)];
    }
}
