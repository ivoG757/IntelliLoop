function conceptRow(index)
{
    return 
    `<div class="concept-item mb-3">
            <div class="mb-3">
                <input name="KeyConcepts[${index}]" id="KeyConcepts_${index}_" class="form-control" />
            </div>

            <button type="button" class="btn btn-outline-danger delete-concept">
                <i class="bi bi-trash"></i>
                Delete concept
            </button>
        </div>`;
}

function noteRow(index)
{
    return 
    `<div class="note-item mb-4">
            <div class="mb-2">
                <input name="Notes[${index}].Title" id="Notes_${index}__Title" class="form-control" />
            </div>

            <div class="mb-2">
                <textarea name="Notes[${index}].Content" id="Notes_${index}__Content" class="form-control"></textarea>
            </div>

            <button type="button" class="btn btn-outline-danger delete-note">
                <i class="bi bi-trash"></i>
                Delete note
            </button>
        </div>`;
}

document.addEventListener("click", function (event)
{
    if (event.target.closest("#add-concept"))
    {
        const list = document.getElementById("concept-list");
        const index = list.querySelectorAll(".concept-item").length;

        list.insertAdjacentHTML("beforeend", conceptRow(index));
        list.lastElementChild.querySelector("input").focus();
        return;
    }

    if (event.target.closest("#add-note"))
    {
        const list = document.querySelector(".notes-list");
        const index = list.querySelectorAll(".note-item").length;

        list.insertAdjacentHTML("beforeend", noteRow(index));
        list.lastElementChild.querySelector("input[name$='.Title']").focus();
        return;
    }

    const deleteNote = event.target.closest(".delete-note");

    if (deleteNote)
    {
        const note = deleteNote.closest(".note-item");
        note.querySelector("input[name$='.Title']").value = "";
        note.querySelector("textarea[name$='.Content']").value = "";
        note.classList.add("d-none");
        return;
    }

    const deleteConcept = event.target.closest(".delete-concept");

    if (deleteConcept)
    {
        deleteConcept.closest(".concept-item").remove();
        reindexConcepts();
    }
});

function reindexConcepts()
{
    document.querySelectorAll(".concept-item").forEach((item, index) =>
    {
        const input = item.querySelector("input");
        const validation = item.querySelector("[data-valmsg-for]");
        input.name = `KeyConcepts[${index}]`;
        input.id = `KeyConcepts_${index}_`;

        if (validation)
        {
            validation.setAttribute("data-valmsg-for", `KeyConcepts[${index}]`);
            validation.id = `KeyConcepts_${index}__validationMessage`;
        }
    });
}